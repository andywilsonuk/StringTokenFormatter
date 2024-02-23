namespace StringTokenFormatter.Impl;

public sealed class StandardTokenCommand : IExpanderCommand
{
    internal StandardTokenCommand() { }

    public void Init(ExpanderContext context) { }

    public void Evaluate(ExpanderContext context)
    {
        if (context.SegmentIterator.Current is not InterpolatedStringTokenSegment tokenSegment) { return; }

        string tokenName = tokenSegment.Token;
        var containerMatch = context.TryMap(tokenName);
        if (context.ConvertValueIfMatched(containerMatch, tokenName, out object? tokenValue))
        {
            context.StringBuilder.AppendTokenValue(context, tokenSegment, tokenValue);
        }
        else
        {
            context.StringBuilder.AppendLiteral(tokenSegment.Raw);
        }
        context.SegmentHandled = true;
    }

    public void Finished(ExpanderContext context) { }
}