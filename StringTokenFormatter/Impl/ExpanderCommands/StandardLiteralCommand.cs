namespace StringTokenFormatter.Impl;

public sealed class StandardLiteralCommand : IExpanderCommand
{
    internal StandardLiteralCommand() { }

    public void Init(ExpanderContext context) { }

    public void Evaluate(ExpanderContext context)
    {
        if (context.SegmentIterator.Current is not InterpolatedStringLiteralSegment rawSegment) { return; }

        context.StringBuilder.AppendLiteral(rawSegment.Raw);
        context.SegmentHandled = true;
    }

    public void Finished(ExpanderContext context) { }
}