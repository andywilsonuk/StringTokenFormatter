namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

public static class InterpolatedStringExtensions {
    /// <summary>
    /// Returns the distinct tokens present within the interpolatedString. Note: this is faithful to the original casing of the token.
    /// </summary>
    public static HashSet<string> Tokens(this InterpolatedString interpolatedString) =>
        new(interpolatedString.Segments.OfType<InterpolatedStringTokenSegment>().Select(x =>
        {
            string tokenName = x.Token;
            if (tokenName.StartsWith(interpolatedString.Settings.ConditionStartToken, StringComparison.Ordinal))
            {
                return tokenName.Substring(interpolatedString.Settings.ConditionStartToken.Length);
            }
            if (tokenName.StartsWith(interpolatedString.Settings.ConditionEndToken, StringComparison.Ordinal))
            {
                return tokenName.Substring(interpolatedString.Settings.ConditionEndToken.Length);
            }
            return tokenName;
        }));
}

public record InterpolatedStringSegment(string Raw);

public record InterpolatedStringTokenSegment(string Raw, string Token, string Alignment, string Format) : InterpolatedStringSegment(Raw);