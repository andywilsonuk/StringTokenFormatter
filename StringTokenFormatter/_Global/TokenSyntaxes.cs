using StringTokenFormatter.Impl;

namespace StringTokenFormatter; 
public static class TokenSyntaxes {

    /// <summary>
    /// Interpolate using: {Token} Escape using: {{
    /// </summary>
    public static ITokenSyntax Curly { get; }

    /// <summary>
    /// Interpolate using: ${Token} Escape using: ${{
    /// </summary>
    public static ITokenSyntax DollarCurly { get; }


    /// <summary>
    /// Interpolate using: (Token) Escape using: ((
    /// </summary>
    public static ITokenSyntax Round { get; }

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $((
    /// </summary>
    public static ITokenSyntax DollarRound { get; }

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $$(
    /// </summary>
    public static ITokenSyntax DollarRoundAlternative { get; }

    /// <summary>
    /// Uses <see cref="Curly"/>
    /// </summary>
    public static ITokenSyntax Default { get; }

    static TokenSyntaxes() {
        Curly = new TokenSyntaxImpl() {
            StartToken = "{",
            EndToken = "}",
            StartTokenEscaped = "{{",
        };

        DollarCurly = new TokenSyntaxImpl() {
            StartToken = "${",
            EndToken = "}",
            StartTokenEscaped = "${{",
        };

        Round = new TokenSyntaxImpl() {
            StartToken = "(",
            EndToken = ")",
            StartTokenEscaped = "((",
        };

        DollarRound = new TokenSyntaxImpl() {
            StartToken = "$(",
            EndToken = ")",
            StartTokenEscaped = "$((",
        };

        DollarRoundAlternative = new TokenSyntaxImpl() {
            StartToken = "$(",
            EndToken = ")",
            StartTokenEscaped = "$$(",
        };

        Default = Curly;
    }

}
