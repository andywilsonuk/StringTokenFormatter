using System.Diagnostics;

namespace StringTokenFormatter.Impl; 
[DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
internal record TokenSyntaxImpl : ITokenSyntax, IGetDebuggerDisplay {
    public string StartToken { get; init; } = string.Empty;
    public string EndToken { get; init; } = string.Empty;
    public string StartTokenEscaped { get; init; } = string.Empty;

    internal string GetDebuggerDisplay() {
        var ret = $@"{StartToken}{{TOKEN}}{EndToken} / {StartTokenEscaped}{{TOKEN}}{EndToken}";
        return ret;
    }

    string IGetDebuggerDisplay.GetDebuggerDisplay() => GetDebuggerDisplay();
}
