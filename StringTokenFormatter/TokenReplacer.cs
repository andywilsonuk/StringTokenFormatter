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

            var mapper = new DictionaryValueMapper(tokenValueDictionary);

            StringBuilder sb = new StringBuilder();
            foreach (var segment in matcher.SplitSegments(input))
            {
                if (segment is TextMatchingSegment textSegment)
                {
                    sb.Append(textSegment.Text);
                    continue;
                }

                var tokenSegment = (TokenMatchingSegment)segment;
                object value = mapper.Map(tokenSegment);
                if (value == null) continue;

                sb.Append(formatter.Format(tokenSegment, value));
            }
            return sb.ToString();
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
    }
}
