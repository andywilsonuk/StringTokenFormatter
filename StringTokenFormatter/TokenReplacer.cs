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
        private IDictionary<string, object> tokenValueDictionary;
        private readonly ITokenMatcher matcher;
        private readonly IValueFormatter formatter;

        public TokenReplacer()
            : this(DefaultMatcher, DefaultFormatter)
        {
        }

        public TokenReplacer(TokenMarkers markers)
            : this(new DefaultTokenMatcher(markers), DefaultFormatter)
        {
        }

        public TokenReplacer(IFormatProvider provider)
            : this(DefaultMatcher, new FormatProviderValueFormatter(provider))
        {
        }

        public TokenReplacer(TokenMarkers markers, IFormatProvider provider)
            : this(new DefaultTokenMatcher(markers), new FormatProviderValueFormatter(provider))
        {
        }

        public TokenReplacer(ITokenMatcher tokenMatcher, IValueFormatter valueFormatter)
        {
            matcher = tokenMatcher;
            formatter = valueFormatter;
        }

        public static ITokenMatcher DefaultMatcher = new DefaultTokenMatcher();
        public static IValueFormatter DefaultFormatter = new FormatProviderValueFormatter();

        public string Format(string input, object tokenValues)
        {
            var mappings = ConvertObjectToDictionary(tokenValues);
            return Format(input, mappings);
        }

        public string Format(string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            tokenValueDictionary = tokenValues;
            NormalisedTokenValues();

            StringBuilder sb = new StringBuilder();
            foreach (var segment in matcher.SplitSegments(input))
            {
                if (segment is TextMatchingSegment textSegment)
                {
                    sb.Append(textSegment.Text);
                    continue;
                }

                var tokenSegment = (TokenMatchingSegment)segment;
                string token = tokenSegment.Token;

                if (!tokenValueDictionary.TryGetValue(token, out object value))
                {
                    sb.Append(tokenSegment.Original);
                    continue;
                }

                ExpandFunction(token);
                ExpandFunctionString(token);
                ExpandLazy(token);
                ExpandLazyString(token);
                value = tokenValueDictionary[token];
                if (value == null) continue;

                sb.Append(formatter.Format(tokenSegment, value));
            }
            return sb.ToString();
        }
        
        private void NormalisedTokenValues()
        {
            var normalisedTokens = new Dictionary<string, object>(tokenValueDictionary.Count, matcher.TokenNameComparer);
            foreach (var pair in tokenValueDictionary)
            {
                string key = pair.Key;
                if (string.IsNullOrEmpty(key)) continue;

                key = matcher.RemoveTokenMarkers(key);
                normalisedTokens.Add(key, pair.Value);
            }
            tokenValueDictionary = normalisedTokens;
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

        private IDictionary<string, object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> mappings = new Dictionary<string, object>(matcher.TokenNameComparer);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                object obj2 = descriptor.GetValue(values);
                mappings.Add(matcher.RemoveTokenMarkers(descriptor.Name), obj2);
            }

            return mappings;
        }
    }
}
