using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            var mappings = this.ConvertObjectToDictionary(tokenValues);
            return this.Format(provider, input, mappings);
        }

        public string Format(IFormatProvider provider, string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            this.FormatPreview(provider, input, tokenValues);
            this.FormatString();
            return workingInput;
        }

        public string FormatPreview(IFormatProvider provider, string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (tokenValues == null || tokenValues.Count == 0) throw new ArgumentNullException("tokenValues", "The token values dictionary cannot be null or empty.");
            this.formatProvider = provider;
            this.workingInput = input;
            this.tokenValueDictionary = tokenValues;

            this.NormalisedTokenValues();
            this.ReplaceTokensWithValues();
            return workingInput;
        }
        
        private void NormalisedTokenValues()
        {
            var normalisedTokens = new Dictionary<string, object>(this.tokenValueDictionary.Count, this.markers.TokenNameComparer);
            foreach (var pair in this.tokenValueDictionary)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                if (!key.StartsWith(this.markers.StartToken)) key = this.FullyQualifiedToken(key);

                normalisedTokens.Add(key, pair.Value);
            }
            this.tokenValueDictionary = normalisedTokens;
        }

        private string FullyQualifiedToken(string token)
        {
            return this.markers.StartToken + token + this.markers.EndToken;
        }

        private void ReplaceTokensWithValues()
        {
            string pattern = @"(.*?)(" + Regex.Escape(this.markers.StartToken) + @".*?" + Regex.Escape(this.markers.EndToken) + @")(.*?)";
            string[] segments = Regex.Split(this.workingInput, pattern, RegexOptions.Singleline);
            StringFormatBuilder sb = new StringFormatBuilder(this.markers);
            foreach (string segment in segments.Where(s => !string.IsNullOrEmpty(s)))
            {
                string segment2 = segment;
                if (segment2.StartsWith(this.markers.StartTokenEscaped))
                {
                    segment2 = segment2.Remove(0, this.markers.StartTokenEscaped.Length);
                    sb.Append(this.markers.StartTokenEscaped);
                }
                if (!segment2.StartsWith(this.markers.StartToken))
                {
                    sb.Append(segment2);
                    continue;
                }

                var tokenPair = this.GetTokenPair(segment2);
                int matchIndex = this.MatchTokenKeyIndex(tokenPair.Key);
                if (matchIndex == MissingToken)
                {
                    sb.Append(segment2);
                    continue;
                }

                sb.AppendToken(matchIndex + tokenPair.Value);

                this.ExpandFunction(tokenPair);
                this.ExpandFunctionString(tokenPair);
                this.ExpandLazy(tokenPair);
                this.ExpandLazyString(tokenPair);
            }
            this.workingInput = sb.ToString();
        }

        private KeyValuePair<string, string> GetTokenPair(string token)
        {
            string strippedToken = token;
            if (token.StartsWith(this.markers.StartToken)) strippedToken = strippedToken.Remove(0, this.markers.StartToken.Length);
            if (token.EndsWith(this.markers.EndToken)) strippedToken = strippedToken.Remove(strippedToken.Length - this.markers.EndToken.Length);
            int index = strippedToken.IndexOfAny(new char[] { ':', ',' });
            if (index == -1) return new KeyValuePair<string, string>(token, null);

            return new KeyValuePair<string, string>(this.FullyQualifiedToken(strippedToken.Substring(0, index)), strippedToken.Substring(index));
        }

        private int MatchTokenKeyIndex(string key)
        {
            int counter = 0;
            foreach (var pair in this.tokenValueDictionary)
            {
                if (this.markers.TokenNameComparer.Equals(pair.Key, key)) return counter;
                counter++;
            }
            return MissingToken;
        }

        private void ExpandFunction(KeyValuePair<string, string> tokenPair)
        {
            Func<string, object> func = this.tokenValueDictionary[tokenPair.Key] as Func<string, object>;
            if (func == null) return;
            this.tokenValueDictionary[tokenPair.Key] = func(tokenPair.Key);
        }

        private void ExpandFunctionString(KeyValuePair<string, string> tokenPair)
        {
            Func<string, string> func = this.tokenValueDictionary[tokenPair.Key] as Func<string, string>;
            if (func == null) return;
            this.tokenValueDictionary[tokenPair.Key] = func(tokenPair.Key);
        }

        private void ExpandLazy(KeyValuePair<string, string> tokenPair)
        {
            Lazy<object> lazy = this.tokenValueDictionary[tokenPair.Key] as Lazy<object>;
            if (lazy == null) return;
            this.tokenValueDictionary[tokenPair.Key] = lazy.Value.ToString();
        }

        private void ExpandLazyString(KeyValuePair<string, string> tokenPair)
        {
            Lazy<string> lazy = this.tokenValueDictionary[tokenPair.Key] as Lazy<string>;
            if (lazy == null) return;
            this.tokenValueDictionary[tokenPair.Key] = lazy.Value;
        }

        private void FormatString()
        {
            try
            {
                this.workingInput = string.Format(this.formatProvider, this.workingInput, this.tokenValueDictionary.Values.ToArray());
            }
            catch (FormatException ex)
            {
                throw new FormatException("Could not format de-tokenised string: " + this.workingInput, ex);
            }
        }

        private IDictionary<string, object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> mappings = new Dictionary<string, object>(this.markers.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object obj2 = descriptor.GetValue(values);
                mappings.Add(this.FullyQualifiedToken(descriptor.Name), obj2);
            }

            return mappings;
        }
    }
}
