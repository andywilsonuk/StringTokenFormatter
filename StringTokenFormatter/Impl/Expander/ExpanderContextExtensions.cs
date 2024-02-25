namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
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

    public static TryGetResult TryMap(this ExpanderContext context, string tokenName)
    {
        var pseudoMatch = context.Commands.TryMapPseudo(context, tokenName);
        if (pseudoMatch.IsSuccess) { return pseudoMatch; }

        var containerMatch = context.Container.TryMap(tokenName);
        if (containerMatch.IsSuccess && containerMatch.Value is ISequenceTokenValueContainer sequence)
        {
            return sequence.TryMap(tokenName, GetLoopIteration(context, sequence));
        }
        return context.Container.TryMap(tokenName);
    }

    public static int GetLoopIteration(this ExpanderContext context, ISequenceTokenValueContainer sequence) =>
        context.Commands.HasCommand<LoopBlockCommand>()
            ? LoopBlockCommand.GetCurrentIteration(context, sequence)
            : throw new ExpanderException($"Loop block command missing from settings");
}