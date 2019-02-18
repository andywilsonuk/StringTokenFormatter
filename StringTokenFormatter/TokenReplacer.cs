using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenReplacer {
        private readonly ITokenMatcher matcher;
        private readonly IValueFormatter formatter;
        private readonly TokenToValueCompositeMapper mapper;

        public TokenReplacer()
            : this(DefaultMatcher, DefaultMappers, DefaultFormatter) {
        }

        public TokenReplacer(TokenMarkers markers)
            : this(new DefaultTokenMatcher(markers), DefaultMappers, DefaultFormatter) {
        }

        public TokenReplacer(IFormatProvider provider)
            : this(DefaultMatcher, DefaultMappers, new FormatProviderValueFormatter(provider)) {
        }

        public TokenReplacer(TokenMarkers markers, IFormatProvider provider)
            : this(new DefaultTokenMatcher(markers), DefaultMappers, new FormatProviderValueFormatter(provider)) {
        }

        public TokenReplacer(ITokenMatcher tokenMatcher, IEnumerable<ITokenToValueMapper> valueMappers, IValueFormatter valueFormatter) {
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            if (valueMappers == null) throw new ArgumentNullException(nameof(valueMappers));
            mapper = new TokenToValueCompositeMapper(valueMappers);
            formatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));
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
            return FormatFromProperties(matcher.SplitSegments(input), propertyContainer);
        }

        public string FormatFromProperties(SegmentedString input, object propertyContainer)
        {
            ITokenValueContainer mapper = new ObjectPropertiesTokenValueContainer(propertyContainer, matcher);
            return MapTokens(input, mapper);
        }

        public string FormatFromDictionary(string input, IDictionary<string, object> tokenValues)
        {
            return FormatFromDictionary(matcher.SplitSegments(input), tokenValues);
        }

        public string FormatFromDictionary(SegmentedString input, IDictionary<string, object> tokenValues)
        {
            if (tokenValues == null) throw new ArgumentNullException(nameof(tokenValues));

            ITokenValueContainer mapper = new DictionaryTokenValueContainer(tokenValues, matcher);
            return MapTokens(input, mapper);
        }


        public string FormatFromDictionary(string input, IDictionary<string, string> tokenValues)
        {
            return FormatFromDictionary(matcher.SplitSegments(input), tokenValues);
        }

        public string FormatFromDictionary(SegmentedString input, IDictionary<string, string> tokenValues)
        {
            if (tokenValues == null) throw new ArgumentNullException(nameof(tokenValues));
            var tokenValues2 = tokenValues.ToDictionary(p => p.Key, p => (object)p.Value);
            return FormatFromDictionary(input, tokenValues2);
        }

        public string FormatFromSingle(string input, string token, object value) {
            return FormatFromSingle(matcher.SplitSegments(input), token, value);
        }

        public string FormatFromSingle(SegmentedString input, string token, object value)
        {
            ITokenValueContainer mapper = new SingleTokenValueContainer(token, value, matcher);
            return MapTokens(input, mapper);
        }

        public string MapTokens(SegmentedString input, ITokenValueContainer container)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (container == null) throw new ArgumentNullException(nameof(container));

            StringBuilder sb = new StringBuilder();
            foreach (var segment in input)
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
