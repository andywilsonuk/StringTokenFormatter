namespace StringTokenFormatter.Impl;

public record InterpolatedString
{
    private static readonly IReadOnlyList<InterpolatedStringSegment> emptySegments =
        Array.AsReadOnly(Array.Empty<InterpolatedStringSegment>());

    private InterpolatedString(IReadOnlyList<InterpolatedStringSegment> segments, IInterpolatedStringSettings settings)
    {
        Segments = segments;
        Settings = settings;
    }

    public IReadOnlyList<InterpolatedStringSegment> Segments { get; }
    public IInterpolatedStringSettings Settings { get; init; }

    /// <summary>
    /// Creates an interpolated string with no segments.
    /// </summary>
    public static InterpolatedString CreateEmpty(IInterpolatedStringSettings settings)
    {
        Guard.NotNull(settings, nameof(settings));
        return new InterpolatedString(emptySegments, settings);
    }

    /// <summary>
    /// Creates an interpolated string with an owned, read-only copy of the supplied segments.
    /// </summary>
    public static InterpolatedString Create(IEnumerable<InterpolatedStringSegment> segments, IInterpolatedStringSettings settings)
    {
        Guard.NotNull(segments, nameof(segments));
        Guard.NotNull(settings, nameof(settings));

        InterpolatedStringSegment[] ownedSegments = [.. segments];
        IReadOnlyList<InterpolatedStringSegment> readOnlySegments = ownedSegments.Length == 0
            ? emptySegments
            : Array.AsReadOnly(ownedSegments);
        return new InterpolatedString(readOnlySegments, settings);
    }
}

public static class InterpolatedStringExtensions
{
    extension(InterpolatedString interpolatedString)
    {
        /// <summary>
        /// Returns the distinct tokens present within the `InterpolatedString`
        /// </summary>
        public HashSet<string> Tokens() =>
            new(interpolatedString.Segments.OfType<InterpolatedStringTokenOnlySegment>().Select(x => x.Token).Where(x => x != string.Empty), interpolatedString.Settings.NameComparer);

        /// <summary>
        /// Combines the `InterpolatedString` segments into a `string` instance
        /// </summary>
        public string ToRawString() =>
            string.Join(null, interpolatedString.Segments.Select(x => x.Raw));
    }
}

public abstract record InterpolatedStringSegment(string Raw);
public record InterpolatedStringLiteralSegment(string Raw) : InterpolatedStringSegment(Raw);

public abstract record InterpolatedStringTokenOnlySegment(string Raw, string Token) : InterpolatedStringSegment(Raw);
public record InterpolatedStringTokenSegment(string Raw, string Token, string Alignment, string Format) : InterpolatedStringTokenOnlySegment(Raw, Token);
public record InterpolatedStringPseudoTokenSegment(string Raw, string Token, string Alignment, string Format) : InterpolatedStringTokenSegment(Raw, Token, Alignment, Format);
public record InterpolatedStringCommandSegment(string Raw, string Command, string Token, string Data) : InterpolatedStringTokenOnlySegment(Raw, Token);

public static class InterpolatedStringSegmentExtensions
{
    extension(InterpolatedStringCommandSegment segment)
    {
        public bool IsCommandEqual(string command) => OrdinalValueHelper.AreEqual(segment.Command, command);
    }

    extension(InterpolatedStringTokenSegment segment)
    {
        public bool IsPseudoEqual(string pseudoTokenCommand) => OrdinalValueHelper.AreEqual(segment.Token, pseudoTokenCommand);
    }
}