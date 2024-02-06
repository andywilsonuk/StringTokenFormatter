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

    /// <summary>
    /// Asserts that the syntax is properly configured
    /// </summary>
    public static TokenSyntax Validate(this TokenSyntax syntax)
    {
        var (start, end, escapedStart) = syntax;
        Guard.NotEmpty(start, nameof(start));
        Guard.NotEmpty(end, nameof(end));
        Guard.NotEmpty(escapedStart, nameof(escapedStart));
        if (new HashSet<string> { start, syntax.End, escapedStart }.Count != 3) { throw new ArgumentException($"Duplicate token marker detected for syntax {syntax}"); }
        return syntax;
    }
}