namespace StringTokenFormatter.Impl;

public sealed class StandardCommand : IExpanderCommand
{
    public void Evaluate(ExpanderContext context)
    {
        if (context.SegmentIterator.Current is InterpolatedStringTokenSegment tokenSegment)
        {
            EvaluateTokenSegment(context, tokenSegment);
            context.SegmentHandled = true;
        }
        if (context.SegmentIterator.Current is InterpolatedStringLiteralSegment rawSegment)
        {
            context.StringBuilder.AppendLiteral(rawSegment.Raw);
            context.SegmentHandled = true;
        }
    }

    private static void EvaluateTokenSegment(ExpanderContext context, InterpolatedStringTokenSegment tokenSegment)
    {
        string tokenName = tokenSegment.Token;
        var containerMatch = context.TryMap(tokenName);
        if (context.ConvertValueIfMatched(containerMatch, tokenName, out object? tokenValue))
        {
            context.StringBuilder.AppendTokenValue(context, tokenSegment, tokenValue);
            return;
        }
        context.StringBuilder.AppendLiteral(tokenSegment.Raw);
    }

    internal StandardCommand() { }
    public void Init(ExpanderContext context) { }
    public void Finished(ExpanderContext context) { }
}