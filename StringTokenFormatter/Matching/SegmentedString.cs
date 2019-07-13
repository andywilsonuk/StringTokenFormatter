using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter {

    public class SegmentedString : IEnumerable<ISegment> {
        private List<ISegment> segments;

        public SegmentedString(IEnumerable<ISegment> allsegments) {
            segments = allsegments.ToList();
        }

        public IEnumerator<ISegment> GetEnumerator() {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }

        public static SegmentedString Parse(string input, ITokenParser parser = default) {
            parser = parser ?? TokenParser.Default;

            return parser.Parse(input);
        }

        public string Format(ITokenValueContainer container, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default) {
            if (container == null) throw new ArgumentNullException(nameof(container));
            converter = converter ?? TokenValueConverter.Default;
            formatter = formatter ?? TokenValueFormatter.Default;

            var sb = new StringBuilder();
            foreach (var segment in this) {
                if (segment is StringSegment textSegment) {
                    sb.Append(textSegment.Original);
                } else if (segment is TokenSegment tokenSegment) {

                    object mappedValue = tokenSegment.Original;

                    if (container.TryMap(tokenSegment, out object value1)) {

                        if (converter.TryConvert(tokenSegment, value1, out object value2)) {
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