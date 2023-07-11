namespace StringTokenFormatter;

public record TokenSyntax(string Start, string End, string EscapedStart);

public static class CommonTokenSyntax
{
    /// <summary>
    /// Interpolate using: {Token} Escape using: {{
    /// </summary>
    public static TokenSyntax Curly => new("{", "}", "{{");

    /// <summary>
    /// Interpolate using: ${Token} Escape using: ${{
    /// </summary>
    public static TokenSyntax DollarCurly => new("${", "}", "${{");

    /// <summary>
    /// Interpolate using: (Token) Escape using: ((
    /// </summary>
    public static TokenSyntax Round => new("(", ")", "((");

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $((
    /// </summary>
    public static TokenSyntax DollarRound => new("$(", ")", "$((");

    /// <summary>
    /// Interpolate using: $(Token) Escape using: $$(
    /// </summary>
    public static TokenSyntax DollarRoundAlternative => new("$(", ")", "$$(");
}
