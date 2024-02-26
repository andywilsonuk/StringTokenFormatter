namespace StringTokenFormatter.Impl;

public sealed class StandardCommand : IExpanderCommand
{
    public void Evaluate(ExpanderContext context)
    {
        if (context.SegmentIterator.Current is InterpolatedStringTokenSegment tokenSegment)
        {
            EvaluateTokenSegment(context, tokenSegment);
            context.SegmentHandled = true;
            return;
        }
        if (context.SegmentIterator.Current is InterpolatedStringLiteralSegment rawSegment)
        {
            context.StringBuilder.AppendLiteral(rawSegment.Raw);
            context.SegmentHandled = true;
        }
    }

    private static void EvaluateTokenSegment(ExpanderContext context, InterpolatedStringTokenSegment segment)
    {
        string tokenName = segment.Token;
        var tokenValueResult = context.TryGetTokenValue(tokenName);
        if (!tokenValueResult.IsSuccess)
        {
            context.StringBuilder.AppendLiteral(segment.Raw);
            return;
        }
        var convertedValue = context.ApplyValueConverter(tokenValueResult.Value, tokenName);
        if (convertedValue == null) { return; }
        AppendTokenValue(context, segment, convertedValue);
    }

    private static void AppendTokenValue(ExpanderContext context, InterpolatedStringTokenSegment tokenSegment, object tokenValue)
    {
        var builder = context.StringBuilder;
        try
        {
            builder.AppendFormat(tokenValue, tokenSegment.Token, tokenSegment.Alignment, tokenSegment.Format);
        }
        catch (FormatException) when (context.Settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveToken)
        {
            builder.AppendLiteral(tokenSegment.Raw);
        }
        catch (FormatException) when (context.Settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveUnformatted)
        {
            builder.AppendUnformatted(tokenValue);
        }
        catch (FormatException ex)
        {
            throw new TokenValueFormatException($"Unable to format token {tokenSegment.Raw}", ex);
        }
    }

    internal StandardCommand() { }
    public void Init(ExpanderContext context) { }
    public void Finished(ExpanderContext context) { }
}