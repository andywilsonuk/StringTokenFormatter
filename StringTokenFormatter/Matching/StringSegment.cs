using System;
using System.Diagnostics;

namespace StringTokenFormatter {

    [DebuggerDisplay(Debugger2.DISPLAY)]
    public class StringSegment : ISegment {
        public StringSegment(string text) {
            Original = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Original { get; }

        public string Evaluate(ITokenValueContainer container, ITokenValueFormatter formatter, ITokenValueConverter converter) {
            return formatter.Format(this, Original, null, null);
        }

        public override string ToString() => Original;

        protected virtual string DebuggerDisplay => $@"""{Original}""";

    }

}