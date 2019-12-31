using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter {

    public class SegmentedString {
        public IReadOnlyCollection<ISegment> Segments { get; private set; }

        public SegmentedString(IEnumerable<ISegment> allsegments) {
            this.Segments = allsegments.ToList().AsReadOnly();
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
            
            foreach (var segment in Segments) {
                sb.Append(segment.Evaluate(container, formatter, converter));
            }

            return sb.ToString();
        }

    }

}