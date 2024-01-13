namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static bool TryGetTokenValue(this ExpanderContext context, string token, out object? value)
    {
        var containerMatch = context.Container.TryMap(token);
        if (containerMatch.IsSuccess)
        {
            var containerValue = containerMatch.Value;
            var converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
            if (converter != default)
            {
                value = converter.Value;
                return true;
            }
            throw new MissingValueConverterException($"No matching value converter found for token '{token}' with container value {containerValue}");
        }
        else if (context.Settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{token}' was not found within the container");
        }
        value = null;
        return false;
    }

    public static void EvaluateCurrentSegment(this ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringTokenSegment tokenSegment)
        {
            context.StringBuilder.AppendTokenValue(context, tokenSegment);
            return;
        }
        if (segment is InterpolatedStringBlockSegment blockSegment)
        {
            // blocks are already handled by commands
            return;
        }
        context.StringBuilder.AppendLiteral(segment.Raw);
    }
}