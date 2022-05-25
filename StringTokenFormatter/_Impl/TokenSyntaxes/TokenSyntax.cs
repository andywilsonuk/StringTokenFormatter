using StringTokenFormatter.Impl;
using System.Diagnostics;

namespace StringTokenFormatter.Impl.TokenSyntaxes {
    [DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
    public record TokenSyntax : ITokenSyntax, IGetDebuggerDisplay {
        public string StartToken { get; init; } = string.Empty;
        public string EndToken { get; init; } = string.Empty;
        public string StartTokenEscaped { get; init; } = string.Empty;

        internal string GetDebuggerDisplay() {
            var ret = $@"{StartToken}{{TOKEN}}{EndToken} / {StartTokenEscaped}{{TOKEN}}{EndToken}";
            return ret;
        }

        string IGetDebuggerDisplay.GetDebuggerDisplay() {
            return GetDebuggerDisplay();
        }
    }

}
