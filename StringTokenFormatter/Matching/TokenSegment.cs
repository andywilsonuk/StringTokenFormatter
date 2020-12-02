using System;
using System.Diagnostics;

namespace StringTokenFormatter {

    [DebuggerDisplay(Debugger2.DISPLAY)]
    public class TokenSegment : ISegment, IMatchedToken {
        public TokenSegment(string original, string token, string? padding, string? format) {
            Original = original ?? throw new ArgumentNullException(nameof(original));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Padding = padding;
            Format = format;
        }

        public string Original { get; }
        public string Token { get; }
        public string? Padding { get; }
        public string? Format { get; }

        public string? Evaluate(ITokenValueContainer container, ITokenValueFormatter formatter, ITokenValueConverter converter) {
            object? mappedValue = Original;

            if (container.TryMap(this, out var value1)) {

                if (converter.TryConvert(this, value1, out var value2)) {
                    mappedValue = value2;
                } else {
                    mappedValue = value1;
                }

            }

            var ret = formatter.Format(this, mappedValue, Padding, Format);

            return ret;
        }

        public override string ToString() => Original;

        protected virtual string DebuggerDisplay => Original;
    }

}