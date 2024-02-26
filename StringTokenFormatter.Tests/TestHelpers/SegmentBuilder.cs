namespace StringTokenFormatter.Tests;

public class SegmentBuilder
{
    private readonly TokenSyntax syntax = StringTokenFormatterSettings.Default.Syntax;
    private readonly List<InterpolatedStringSegment> innerList = [];

    private SegmentBuilder Add(InterpolatedStringSegment segment)
    {
        innerList.Add(segment);
        return this;
    }

    public SegmentBuilder Literal(string raw) =>
        Add(new InterpolatedStringLiteralSegment(raw));

    public SegmentBuilder Token(string token, string alignment, string format) =>
        Add(new InterpolatedStringTokenSegment(CreateRaw(token, alignment, format), token, alignment, format));

    public SegmentBuilder Command(string command, string token, string data) =>
        Add(new InterpolatedStringCommandSegment(CreateRaw($"{Constants.CommandPrefix}{command}", token, data), command, token, data));

    public SegmentBuilder Pseudo(string token, string alignment, string format) =>
        Add(new InterpolatedStringPseudoTokenSegment(CreateRaw($"{Constants.PseudoPrefix}{token}", alignment, format), Constants.PseudoPrefix + token, alignment, format));

    public List<InterpolatedStringSegment> Build() => innerList;

    private string CreateRaw(string part1, string part2, string part3) => syntax.Tokenize($"{part1}{(part2.Length == 0 ? "" : ",")}{part2}{(part3.Length == 0 ? "" : ":")}{part3}");
}