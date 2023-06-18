using System.Diagnostics;
using System.Text;

namespace StringTokenFormatter.Impl;


[DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
internal class InterpolatedStringImpl : IInterpolatedString, IGetDebuggerDisplay {
    protected IInterpolatedStringSegment[] Segments { get; }

    IEnumerable<IInterpolatedStringSegment> IInterpolatedString.Segments => Segments;

    public InterpolatedStringImpl(IEnumerable<IInterpolatedStringSegment> Segments) {
        this.Segments = Segments.ToArray();
    }

    public string FormatContainer(ITokenValueContainer container, ITokenValueConverter ValueConverter, ITokenValueFormatter ValueFormatter) {

        var sb = new StringBuilder();
        
        foreach (var segment in Segments) {
            sb.Append(segment.Evaluate(container, ValueConverter, ValueFormatter));
        }

        return sb.ToString();
    }

    internal string GetDebuggerDisplay() {
        var sb = new StringBuilder();
        foreach (var segment in Segments) {
            sb.Append(segment.GetDebuggerDisplay());
        }

        return sb.ToString();
    }

    string IGetDebuggerDisplay.GetDebuggerDisplay() => GetDebuggerDisplay();
}