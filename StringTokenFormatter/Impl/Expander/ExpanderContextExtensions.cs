namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static bool TryGetTokenValue(this ExpanderContext context, string token, out object? value) =>
        TryGetTokenValue(context, context.Container, token, out value);

    public static bool TryGetTokenValue(this ExpanderContext context, ITokenValueContainer container, string token, out object? value)
    {
        var containerMatch = container.TryMap(token);
        return ConvertOnSuccess(context, containerMatch, token, out value);
    }

    public static bool ConvertOnSuccess(this ExpanderContext context, TryGetResult containerMatch, string token, out object? value)
    {
        if (containerMatch.IsSuccess)
        {
            value = ConvertValue(context, token, containerMatch.Value);
            return true;
        }
        else if (context.Settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{token}' was not found within the container");
        }
        value = null;
        return false;
    }

    public static object? ConvertValue(this ExpanderContext context, string token, object? containerValue)
    {
        var converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
        return converter != default
            ? converter.Value
            : throw new MissingValueConverterException($"No matching value converter found for token '{token}' with container value {containerValue}");
    }

    public static void EvaluateCurrentSegment(this ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringTokenSegment tokenSegment)
        {
            context.StringBuilder.AppendTokenValue(context, tokenSegment);
            return;
        }
        if (segment is InterpolatedStringBlockSegment)
        {
            // blocks are already handled by commands
            return;
        }
        context.StringBuilder.AppendLiteral(segment.Raw);
    }
}