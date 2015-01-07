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
        private const string Pattern = @"(.*?)(\{.*?\})(.*?)";
        private const int MissingToken = -1;
        private const char StartToken = '{';
        private const char EndToken = '}';
        private const string StartTokenEscaped = "{{";
        private const string EndTokenEscape = "}}";
        private IDictionary<string, object> values;
        private IFormatProvider formatProvider;
        private string workingInput;

        public string Format(IFormatProvider provider, string input, object tokenValues)
        {
            var mappings = this.ConvertObjectToDictionary(tokenValues);
            return this.Format(provider, input, mappings);
        }

        public string Format(IFormatProvider provider, string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (tokenValues == null || tokenValues.Count == 0) throw new ArgumentNullException("tokenValues", "The token values dictionary cannot be null or empty.");
            this.formatProvider = provider;
            this.workingInput = input;
            this.values = tokenValues;

            this.NormalisedTokenValues();
            this.ReplaceTokensWithValues();
            this.FormatString();
            return workingInput;
        }

        private void NormalisedTokenValues()
        {
            var normalisedTokens = new Dictionary<string, object>(this.values.Count);
            foreach (var pair in this.values)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                if (key[0] != StartToken) key = this.FormattedToken(key);

                normalisedTokens.Add(key, pair.Value);
            }
            this.values = normalisedTokens;
        }

        private string FormattedToken(string key)
        {
            return StartToken + key + EndToken;
        }

        private void ReplaceTokensWithValues()
        {
            string[] segments = Regex.Split(this.workingInput, Pattern, RegexOptions.Singleline);
            StringBuilder sb = new StringBuilder();
            foreach (string segment in segments.Where(s => !string.IsNullOrEmpty(s)))
            {
                string segment2 = segment;
                if (segment2.StartsWith(StartTokenEscaped))
                {
                    sb.Append(StartTokenEscaped);
                    segment2 = segment2.Remove(0, StartTokenEscaped.Length);
                }
                if (segment2[0] != StartToken)
                {
                    sb.Append(segment2);
                    continue;
                }

                var tokenPair = this.GetTokenPair(segment2);
                int matchIndex = this.MatchTokenKeyIndex(tokenPair.Key);
                if (matchIndex == MissingToken)
                {
                    sb.Append(this.FormattedToken(segment2));
                    continue;
                }

                sb.Append(this.FormattedToken(matchIndex.ToString() + tokenPair.Value));
            }
            this.workingInput = sb.ToString();
        }

        private KeyValuePair<string, string> GetTokenPair(string token)
        {
            string strippedToken = token.Trim(StartToken, EndToken);
            int index = strippedToken.IndexOfAny(new char[] { ':', ',' });
            if (index == -1) return new KeyValuePair<string, string>(token, null);

            return new KeyValuePair<string, string>(this.FormattedToken(strippedToken.Substring(0, index)), strippedToken.Substring(index));
        }

        private int MatchTokenKeyIndex(string key)
        {
            int counter = 0;
            foreach (var pair in this.values)
            {
                if (pair.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)) return counter;
                counter++;
            }
            return MissingToken;
        }

        private void FormatString()
        {
            try
            {
                this.workingInput = string.Format(this.formatProvider, this.workingInput, this.values.Values.ToArray());
            }
            catch (FormatException ex)
            {
                throw new FormatException("Could not format de-tokenised string: " + this.workingInput, ex);
            }
        }

        private IDictionary<string, object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> mappings = new Dictionary<string, object>();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object obj2 = descriptor.GetValue(values);
                mappings.Add(this.FormattedToken(descriptor.Name), obj2);
            }

            return mappings;
        }
    }
}
