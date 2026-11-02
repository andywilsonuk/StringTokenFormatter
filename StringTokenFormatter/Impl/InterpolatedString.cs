namespace StringTokenFormatter.Impl;

public record InterpolatedString(IReadOnlyCollection<InterpolatedStringSegment> Segments, IInterpolatedStringSettings Settings);

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