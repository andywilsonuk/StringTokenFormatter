namespace StringTokenFormatter;

public record TokenSyntax(string Start, string End, string EscapedStart);

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
