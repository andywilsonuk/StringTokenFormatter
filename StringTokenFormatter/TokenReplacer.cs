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
        private readonly ITokenMatcher matcher;
        private readonly IValueFormatter formatter;
        private readonly TokenToValueCompositeMapper mapper;

        public TokenReplacer()
            : this(DefaultMatcher, DefaultFormatter, DefaultMappers)
        {
        }

        public TokenReplacer(TokenMarkers markers)
            : this(new DefaultTokenMatcher(markers), DefaultFormatter, DefaultMappers)
        {
        }

        public TokenReplacer(IFormatProvider provider)
            : this(DefaultMatcher, new FormatProviderValueFormatter(provider), DefaultMappers)
        {
        }

        public TokenReplacer(TokenMarkers markers, IFormatProvider provider)
            : this(new DefaultTokenMatcher(markers), new FormatProviderValueFormatter(provider), DefaultMappers)
        {
        }

        public TokenReplacer(ITokenMatcher tokenMatcher, IValueFormatter valueFormatter, IEnumerable<ITokenToValueMapper> valueMappers)
        {
            matcher = tokenMatcher;
            formatter = valueFormatter;
            mapper = new TokenToValueCompositeMapper(valueMappers);
        }

        public static ITokenMatcher DefaultMatcher = new DefaultTokenMatcher();
        public static IValueFormatter DefaultFormatter = new FormatProviderValueFormatter();
        public static IEnumerable<ITokenToValueMapper> DefaultMappers = new ITokenToValueMapper[]
        {
            new TokenToNullValueMapper(),
            new TokenToLazyStringValueMapper(),
            new TokenToLazyObjectValueMapper(),
            new TokenToFunctionStringNoInputValueMapper(),
            new TokenToFunctionObjectNoInputValueMapper(),
            new TokenToFunctionStringValueMapper(),
            new TokenToFunctionObjectValueMapper(),
        };

        public string FormatFromProperties(string input, object propertyContainer)
        {
            if (string.IsNullOrEmpty(input)) return input;

            ITokenValueContainer mapper = new ObjectPropertiesTokenValueContainer(propertyContainer, matcher);
            return MapTokens(input, mapper);
        }

        public string FormatFromDictionary(string input, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (tokenValues == null) throw new ArgumentNullException(nameof(tokenValues));

            ITokenValueContainer mapper = new DictionaryTokenValueContainer(tokenValues, matcher);
            return MapTokens(input, mapper);
        }

        public string FormatFromDictionary(string input, IDictionary<string, string> tokenValues)
        {
            var tokenValues2 = tokenValues.ToDictionary(p => p.Key, p => (object)p.Value);
            return FormatFromDictionary(input, tokenValues2);
        }

        private string MapTokens(string input, ITokenValueContainer container)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var segment in matcher.SplitSegments(input))
            {
                if (segment is TextMatchingSegment textSegment)
                {
                    sb.Append(textSegment.Text);
                    continue;
                }

                var tokenSegment = (TokenMatchingSegment)segment;
                object mappedValue = tokenSegment.Original;
                if (container.TryMap(tokenSegment, out object value))
                {
                    mappedValue = mapper.TryMap(tokenSegment, value, out object value2) ? value2 : value;
                }

                sb.Append(formatter.Format(tokenSegment, mappedValue));
            }
            return sb.ToString();
        }
    }
}
