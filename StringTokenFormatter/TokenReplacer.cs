using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringTokenFormatter
{
    public class TokenReplacer
    {
        private TokenMarkers markers;
        private const int MissingToken = -1;
        private IDictionary<string, object> tokenValueDictionary;
        private IFormatProvider formatProvider;
        private string workingInput;

        public TokenReplacer(TokenMarkers markers)
        {
            this.markers = markers;
        }

        public TokenReplacer()
            : this(new DefaultTokenMarkers())
        {
        }

        public string Format(IFormatProvider provider, string input, object tokenValues)
        {
            var mappings = ConvertObjectToDictionary(tokenValues);
            return Format(provider, input, mappings);
        }

        public string Format(IFormatProvider provider, string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            tokenValueDictionary = tokenValues;
            NormalisedTokenValues();

            var matcher = new DefaultTokenMatcher(markers);
            StringBuilder sb = new StringBuilder();
            foreach (var segment in matcher.SplitSegments(input))
            {
                if (segment is TextMatchingSegment textSegment)
                {
                    sb.Append(textSegment.Text);
                    continue;
                }

                var tokenSegment = (TokenMatchingSegment)segment;
                string fullToken = markers.StartToken + tokenSegment.Token + markers.EndToken;


                if (!tokenValueDictionary.TryGetValue(fullToken, out object value))
                { 
                    sb.Append(tokenSegment.Original);
                    continue;
                }

                ExpandFunction(fullToken);
                ExpandFunctionString(fullToken);
                ExpandLazy(fullToken);
                ExpandLazyString(fullToken);
                value = tokenValueDictionary[fullToken];
                if (value == null) continue;

                string padding = string.IsNullOrEmpty(tokenSegment.Padding) ? "0" : tokenSegment.Padding;

                sb.AppendFormat(provider, "{0," + padding + ":" + tokenSegment.Format + "}", value);
            }
            return sb.ToString();
        }

        public string FormatPreview(IFormatProvider provider, string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (tokenValues == null || tokenValues.Count == 0) throw new ArgumentNullException("tokenValues", "The token values dictionary cannot be null or empty.");
            formatProvider = provider;
            workingInput = input;
            tokenValueDictionary = tokenValues;

            NormalisedTokenValues();
            ReplaceTokensWithValues();
            return workingInput;
        }
        
        private void NormalisedTokenValues()
        {
            var normalisedTokens = new Dictionary<string, object>(tokenValueDictionary.Count, markers.TokenNameComparer);
            foreach (var pair in tokenValueDictionary)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                if (!key.StartsWith(markers.StartToken)) key = FullyQualifiedToken(key);

                normalisedTokens.Add(key, pair.Value);
            }
            tokenValueDictionary = normalisedTokens;
        }

        private string FullyQualifiedToken(string token)
        {
            return markers.StartToken + token + markers.EndToken;
        }

        private void ReplaceTokensWithValues()
        {
            string pattern = BuildPattern();
            string[] segments = Regex.Split(workingInput, pattern, RegexOptions.Singleline);
            StringFormatBuilder sb = new StringFormatBuilder(markers);
            foreach (string segment in segments.Where(s => !string.IsNullOrEmpty(s)))
            {
                string segment2 = segment;
                if (segment2.StartsWith(markers.StartTokenEscaped))
                {
                    segment2 = segment2.Remove(0, markers.StartTokenEscaped.Length);
                    sb.Append(markers.StartTokenEscaped);
                }
                if (!segment2.StartsWith(markers.StartToken))
                {
                    sb.Append(segment2);
                    continue;
                }

                var tokenPair = GetTokenPair(segment2);
                int matchIndex = MatchTokenKeyIndex(tokenPair.Key);
                if (matchIndex == MissingToken)
                {
                    sb.Append(segment2);
                    continue;
                }

                sb.AppendToken(matchIndex + tokenPair.Value);

                ExpandFunction(tokenPair.Key);
                ExpandFunctionString(tokenPair.Key);
                ExpandLazy(tokenPair.Key);
                ExpandLazyString(tokenPair.Key);
            }
            workingInput = sb.ToString();
        }

        private string BuildPattern()
        {
            string escapedStartToken = Regex.Escape(markers.StartToken);
            string escapedEndToken = Regex.Escape(markers.EndToken);
            string pattern = $"(.*?)({escapedStartToken}[^{escapedStartToken}{escapedEndToken}]*?{escapedEndToken})(.*?)";
            return pattern;
        }

        private KeyValuePair<string, string> GetTokenPair(string token)
        {
            string strippedToken = token;
            if (token.StartsWith(markers.StartToken)) strippedToken = strippedToken.Remove(0, markers.StartToken.Length);
            if (token.EndsWith(markers.EndToken)) strippedToken = strippedToken.Remove(strippedToken.Length - markers.EndToken.Length);
            int index = strippedToken.IndexOfAny(new char[] { ':', ',' });
            if (index == -1) return new KeyValuePair<string, string>(token, null);

            return new KeyValuePair<string, string>(FullyQualifiedToken(strippedToken.Substring(0, index)), strippedToken.Substring(index));
        }

        private int MatchTokenKeyIndex(string key)
        {
            int counter = 0;
            foreach (var pair in tokenValueDictionary)
            {
                if (markers.TokenNameComparer.Equals(pair.Key, key)) return counter;
                counter++;
            }
            return MissingToken;
        }

        private void ExpandFunction(string token)
        {
            Func<string, object> func = tokenValueDictionary[token] as Func<string, object>;
            if (func == null) return;
            tokenValueDictionary[token] = func(token);
        }

        private void ExpandFunctionString(string token)
        {
            Func<string, string> func = tokenValueDictionary[token] as Func<string, string>;
            if (func == null) return;
            tokenValueDictionary[token] = func(token);
        }

        private void ExpandLazy(string token)
        {
            Lazy<object> lazy = tokenValueDictionary[token] as Lazy<object>;
            if (lazy == null) return;
            tokenValueDictionary[token] = lazy.Value.ToString();
        }

        private void ExpandLazyString(string token)
        {
            Lazy<string> lazy = tokenValueDictionary[token] as Lazy<string>;
            if (lazy == null) return;
            tokenValueDictionary[token] = lazy.Value;
        }

        private void FormatString()
        {
            try
            {
                workingInput = string.Format(formatProvider, workingInput, tokenValueDictionary.Values.ToArray());
            }
            catch (FormatException ex)
            {
                throw new FormatException("Could not format de-tokenised string: " + workingInput, ex);
            }
        }

        private IDictionary<string, object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> mappings = new Dictionary<string, object>(markers.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object obj2 = descriptor.GetValue(values);
                mappings.Add(FullyQualifiedToken(descriptor.Name), obj2);
            }

            return mappings;
        }
    }
}
