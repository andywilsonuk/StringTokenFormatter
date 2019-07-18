using System;

namespace StringTokenFormatter {

    public class StringSegment : ISegment {
        public StringSegment(string text) {
            Original = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Original { get; }
        public override string ToString() => Original;
    }

}