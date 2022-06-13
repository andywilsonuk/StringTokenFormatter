using System.Diagnostics;

namespace StringTokenFormatter.Impl {

    [DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
    internal record InterpolatedStringSegmentTokenImpl : IInterpolatedStringSegmentToken, IGetDebuggerDisplay {
        public InterpolatedStringSegmentTokenImpl(string original, string token, string? padding, string? format) {
            Original = original;
            Token = token;
            Padding = padding;
            Format = format;
        }

        public string Original { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
        public string? Padding { get; init; }
        public string? Format { get; init; }

        public string? Evaluate(ITokenValueContainer container, ITokenValueConverter converter, ITokenValueFormatter formatter) {
            object? mappedValue = Original;


            var Mapped = container.TryMap(this);
            if (Mapped.IsSuccess) {
                if (converter.TryConvert(this, Mapped.Value, out var value2)) {
                    mappedValue = value2;
                }
                else {
                    mappedValue = Mapped.Value;
                }
            }

            var ret = formatter.Format(this, mappedValue, Padding, Format);

            return ret;
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