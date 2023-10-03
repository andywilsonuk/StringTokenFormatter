namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

public static class InterpolatedStringExtensions {
    public static HashSet<string> Tokens(InterpolatedString interpolatedString) => interpolatedString.Segments.OfType<InterpolatedStringTokenSegment>().Select(x => x.Token).ToHashSet();
}

public record InterpolatedStringSegment(string Raw);

public record InterpolatedStringTokenSegment(string Raw, string Token, string? Padding, string? Format) : InterpolatedStringSegment(Raw);