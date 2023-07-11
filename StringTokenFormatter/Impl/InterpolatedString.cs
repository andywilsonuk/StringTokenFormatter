namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IIInterpolatedStringSettings Settings)
{
    public HashSet<string> Tokens => Segments.OfType<InterpolatedStringTokenSegment>().Select(x => x.Token).ToHashSet();
}

public record InterpolatedStringSegment(string Raw);

public record InterpolatedStringTokenSegment(string Raw, string Token, string? Padding, string? Format) : InterpolatedStringSegment(Raw);