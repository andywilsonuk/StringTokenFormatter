using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter {
    public class SegmentedString : IEnumerable<IMatchingSegment> {
        private List<IMatchingSegment> segments;

        public SegmentedString(IEnumerable<IMatchingSegment> allsegments)
        {
            segments = allsegments.ToList();
        }

        public IEnumerator<IMatchingSegment> GetEnumerator() {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return segments.GetEnumerator();
        }

        public static SegmentedString Create(string input) {
            return Create(input, TokenReplacer.DefaultMatcher);
        }

        public static SegmentedString Create(string input, ITokenMatcher Matcher) {
            return Matcher.SplitSegments(input);
        }
    }
}
