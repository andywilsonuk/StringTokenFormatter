namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static bool TryGetTokenValue(this ExpanderContext context, string token, out object? value)
    {
        var containerMatch = context.Container.TryMap(token);
        return ConvertValueIfMatched(context, containerMatch, token, out value);
    }

    public static bool ConvertValueIfMatched(this ExpanderContext context, TryGetResult containerMatch, string token, out object? value)
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

    private static object? ConvertValue(ExpanderContext context, string token, object? containerValue)
    {
        var converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
        return converter != default
            ? converter.Value
            : throw new MissingValueConverterException($"No matching value converter found for token '{token}'");
    }

    
    public static bool TryGetSequence(this ExpanderContext context, string token, [NotNullWhen(true)] out ISequenceTokenValueContainer? list)
    {
        var containerMatch = context.Container.TryMap(token);
        list = containerMatch.IsSuccess && containerMatch.Value is ISequenceTokenValueContainer listValue ? listValue : null;
        return list != null;
    }

    public static int GetLoopIteration(this ExpanderContext context, ISequenceTokenValueContainer sequence)
    {
        if (!context.Settings.BlockCommands.OfType<LoopBlockCommand>().Any())
        {
            throw new ExpanderException($"Loop block command missing from settings");
        }
        return LoopBlockCommand.GetCurrentIteration(context, sequence);
    }

    public static void EvaluateSegment(this ExpanderContext context, InterpolatedStringSegment segment)
    {
        switch (segment)
        {
            case InterpolatedStringTokenSegment tokenSegment:
                context.StringBuilder.AppendTokenValue(context, tokenSegment);
                return;
            case InterpolatedStringBlockSegment:
                // blocks are already handled by commands
                return;
            case InterpolatedStringLiteralSegment rawSegment:
                context.StringBuilder.AppendLiteral(rawSegment.Raw);
                return;
            default: throw new ExpanderException($"Unknown segment type {segment}");
        }
    }
}