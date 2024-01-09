namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

public static class InterpolatedStringExtensions {
    /// <summary>
    /// Returns the distinct tokens present within the interpolatedString. Note: this is faithful to the original casing of the token.
    /// </summary>
    public static HashSet<string> Tokens(this InterpolatedString interpolatedString)
    {
        string conditionStartToken = interpolatedString.Settings.ConditionStartToken;
        string conditionEndToken = interpolatedString.Settings.ConditionEndToken;

        return new(interpolatedString.Segments.OfType<InterpolatedStringTokenSegment>().Select(x =>
        {
            string tokenName = x.Token;
            if (tokenName.StartsWith(conditionStartToken, StringComparison.Ordinal))
            {
                return tokenName.Substring(conditionStartToken.Length);
            }
            if (tokenName.StartsWith(conditionEndToken, StringComparison.Ordinal))
            {
                return tokenName.Substring(conditionEndToken.Length);
            }
            return tokenName;
        }));
    }
}

public abstract record InterpolatedStringSegment(string Raw);
public record InterpolatedStringLiteralSegment(string Raw) : InterpolatedStringSegment(Raw);

public abstract record InterpolatedStringTokenOnlySegment(string Raw, string Token) : InterpolatedStringSegment(Raw);
public record InterpolatedStringTokenSegment(string Raw, string Token, string Alignment, string Format) : InterpolatedStringTokenOnlySegment(Raw, Token);
public record InterpolatedStringBlockSegment(string Raw, string Command, string Token, string Data) : InterpolatedStringTokenOnlySegment(Raw, Token)
{
    public static StringComparer CommandComparer => StringComparer.Ordinal;
}

public static class InterpolatedStringBlockSegmentExtensions
{
    public static bool IsCommand(this InterpolatedStringBlockSegment block, string command) => InterpolatedStringBlockSegment.CommandComparer.Equals(block.Command, command);
    
}