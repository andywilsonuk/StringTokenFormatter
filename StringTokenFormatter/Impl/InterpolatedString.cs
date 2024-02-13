namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

public static class InterpolatedStringExtensions
{
    /// <summary>
    /// Returns the distinct tokens present within the `InterpolatedString`
    /// </summary>
    public static HashSet<string> Tokens(this InterpolatedString interpolatedString) =>
        new(interpolatedString.Segments.OfType<InterpolatedStringTokenOnlySegment>().Select(x => x.Token).Where(x => x != string.Empty), interpolatedString.Settings.NameComparer);
}

public abstract record InterpolatedStringSegment(string Raw);
public record InterpolatedStringLiteralSegment(string Raw) : InterpolatedStringSegment(Raw);

public abstract record InterpolatedStringTokenOnlySegment(string Raw, string Token) : InterpolatedStringSegment(Raw);
public record InterpolatedStringTokenSegment(string Raw, string Token, string Alignment, string Format) : InterpolatedStringTokenOnlySegment(Raw, Token);
public record InterpolatedStringCommandSegment(string Raw, string Command, string Token, string Data) : InterpolatedStringTokenOnlySegment(Raw, Token)
{
    public static StringComparer CommandComparer => StringComparer.Ordinal;
}

public static class InterpolatedStringCommandSegmentExtensions
{
    public static bool IsCommand(this InterpolatedStringCommandSegment segment, string command) => InterpolatedStringCommandSegment.CommandComparer.Equals(segment.Command, command);

}