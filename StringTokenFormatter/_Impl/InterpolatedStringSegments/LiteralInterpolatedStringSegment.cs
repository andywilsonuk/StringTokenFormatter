using System.Diagnostics;

namespace StringTokenFormatter.Impl; 

[DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
internal record InterpolatedStringSegmentLiteralImpl : IInterpolatedStringSegmentLiteral, IGetDebuggerDisplay {

    public string Original { get; init; } = string.Empty;

    public InterpolatedStringSegmentLiteralImpl(string text) {
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