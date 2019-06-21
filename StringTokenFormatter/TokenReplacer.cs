using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenReplacer
    {
        private readonly ITokenMatcher matcher;
        private readonly IValueFormatter formatter;
        private readonly TokenToValueCompositeMapper mapper;

        public TokenReplacer()
            : this(DefaultMatcher, DefaultMappers, DefaultFormatter)
        {
        }

        public TokenReplacer(ITokenMarkers markers)
            : this(new DefaultTokenMatcher(markers), DefaultMappers, DefaultFormatter)
        {
        }

        public TokenReplacer(IFormatProvider provider)
            : this(DefaultMatcher, DefaultMappers, new FormatProviderValueFormatter(provider))
        {
        }

        public TokenReplacer(ITokenMarkers markers, IFormatProvider provider)
            : this(new DefaultTokenMatcher(markers), DefaultMappers, new FormatProviderValueFormatter(provider))
        {
        }

        public TokenReplacer(ITokenMatcher tokenMatcher, IEnumerable<ITokenToValueMapper> valueMappers, IValueFormatter valueFormatter)
        {
            matcher = tokenMatcher ?? throw new ArgumentNullException(nameof(tokenMatcher));
            if (valueMappers == null) throw new ArgumentNullException(nameof(valueMappers));
            mapper = new TokenToValueCompositeMapper(valueMappers);
            formatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));
        }

        public static ITokenMatcher DefaultMatcher = new DefaultTokenMatcher();
        public static IValueFormatter DefaultFormatter = new FormatProviderValueFormatter();
        public static IEnumerable<ITokenToValueMapper> DefaultMappers = new ITokenToValueMapper[]
        {
            new TokenToPrimitiveValueMapper(),
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
            ITokenValueContainer mapper = new ObjectPropertiesTokenValueContainer(propertyContainer, matcher);
            return FormatFromContainer(input, mapper);
        }

        public string FormatFromProperties<T>(string input, T propertyContainer) {
            ITokenValueContainer mapper = new ObjectPropertiesTokenValueContainer<T>(propertyContainer, matcher);
            return FormatFromContainer(input, mapper);
        }

        public string FormatFromDictionary(string input, IDictionary<string, object> tokenValues)
        {
            if (tokenValues == null) throw new ArgumentNullException(nameof(tokenValues));

            ITokenValueContainer mapper = new DictionaryTokenValueContainer(tokenValues, matcher);
            return FormatFromContainer(input, mapper);
        }

        public string FormatFromDictionary(string input, IDictionary<string, string> tokenValues)
        {
            if (tokenValues == null) throw new ArgumentNullException(nameof(tokenValues));
            var tokenValues2 = tokenValues.ToDictionary(p => p.Key, p => (object)p.Value);
            return FormatFromDictionary(input, tokenValues2);
        }

        public string FormatFromSingle(string input, string token, object value)
        {
            ITokenValueContainer mapper = new SingleTokenValueContainer(token, value, matcher);
            return FormatFromContainer(input, mapper);
        }

        public string FormatFromContainer(string input, ITokenValueContainer container)
        {
            var segmentedString = matcher.SplitSegments(input);
            return FormatFromContainer(segmentedString, container);
        }

        public string FormatFromContainer(SegmentedString segmentedString, ITokenValueContainer container)
        {
            if (segmentedString == null) throw new ArgumentNullException(nameof(segmentedString));
            if (container == null) throw new ArgumentNullException(nameof(container));

            var sb = new StringBuilder();
            foreach (var segment in segmentedString)
            {
                if (segment is TextMatchingSegment textSegment) {
                    sb.Append(textSegment.Original);
                } else if (segment is TokenMatchingSegment tokenSegment) {

                    object mappedValue = tokenSegment.Original;

                    if (container.TryMap(tokenSegment, out object value1)) {

                        if (mapper.TryMap(tokenSegment, value1, out object value2)) {
                            mappedValue = value2;
                        } else {
                            mappedValue = value1;
                        }

                    }
                    
                    sb.Append(formatter.Format(tokenSegment, mappedValue));
                }
            }
            return sb.ToString();
        }
    }
}
