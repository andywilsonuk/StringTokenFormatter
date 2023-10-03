namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

public static class InterpolatedStringExtensions {
    /// <summary>
    /// Returns the distinct tokens present within the interpolatedString. Note: this is faithful to the original casing of the token.
    /// </summary>
    public static HashSet<string> Tokens(this InterpolatedString interpolatedString) =>
        interpolatedString.Segments.OfType<InterpolatedStringTokenSegment>().Select(x => x.Token).ToHashSet();
}

public record InterpolatedStringSegment(string Raw);

public record InterpolatedStringTokenSegment(string Raw, string Token, string Padding, string Format) : InterpolatedStringSegment(Raw);