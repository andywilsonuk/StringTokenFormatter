using StringTokenFormatter.Impl;
using System;
using System.Diagnostics;

namespace StringTokenFormatter.Impl.InterpolatedStringSegments {

    [DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
    public record LiteralInterpolatedStringSegment : IInterpolatedStringSegment, IGetDebuggerDisplay {

        public string Original { get; init; } = string.Empty;

        public LiteralInterpolatedStringSegment(string text) {
            Original = text;
        }

        public string? Evaluate(ITokenValueContainer container, ITokenValueConverter converter, ITokenValueFormatter formatter) {
            return formatter.Format(this, Original, null, null);
        }

        public override string ToString() => Original;

        internal string GetDebuggerDisplay() {
            return Original;
        }

        string IGetDebuggerDisplay.GetDebuggerDisplay() {
            return GetDebuggerDisplay();
        }
    }

}