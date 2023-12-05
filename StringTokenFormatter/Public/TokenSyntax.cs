namespace StringTokenFormatter;

public readonly record struct TokenSyntax(string Start, string End, string EscapedStart);

public static class CommonTokenSyntax
{
    /// <summary>
    /// Interpolate using: {Token} Escape using: {{
    /// </summary>
    public static TokenSyntax Curly { get; } = new("{", "}", "{{");

    /// <summary>
    /// Interpolate using: ${Token} Escape using: ${{
    /// </summary>
    public static TokenSyntax DollarCurly { get; } = new("${", "}", "${{");

    /// <summary>
    /// Interpolate using: (Token) Escape using: ((
    /// </summary>
    public static TokenSyntax Round { get; } = new("(", ")", "((");

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $((
    /// </summary>
    public static TokenSyntax DollarRound { get; } = new("$(", ")", "$((");

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $$(
    /// </summary>
    public static TokenSyntax DollarRoundAlternative { get; } = new("$(", ")", "$$(");
}

public static class TokenSyntaxExtensions
{
    /// <summary>
    /// Returns the token name wrapped within the start and end syntax.
    /// </summary>
    public static string Tokenize(this TokenSyntax syntax, string tokenName) => $"{syntax.Start}{tokenName}{syntax.End}";
}