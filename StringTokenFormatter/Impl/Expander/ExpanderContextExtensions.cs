namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static TryGetResult TryGetTokenValue(this ExpanderContext context, string tokenName)
    {
        var pseudoMatch = context.Commands.TryMapPseudo(context, tokenName);
        if (pseudoMatch.IsSuccess) { return pseudoMatch; }

        var containerMatch = context.Container.TryMap(tokenName);
        if (containerMatch.IsSuccess)
        {
            if (containerMatch.Value is not ISequenceTokenValueContainer sequence) { return containerMatch; }

            var sequenceMatch = sequence.TryMap(tokenName, GetLoopIteration(context, sequence));
            if (sequenceMatch.IsSuccess) { return sequenceMatch; }

        }
        var replacementSettings = context.Settings as IUnresolvedTokenReplacementSettings;
        var defaultBehavior = context.Settings.UnresolvedTokenBehavior;
        var behavior = replacementSettings?.UnresolvedTokenReplacementFormatter(tokenName, defaultBehavior) ?? defaultBehavior;
        Guard.IsDefined(behavior, nameof(IUnresolvedTokenReplacementSettings.UnresolvedTokenReplacementFormatter));
        if (behavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{tokenName}' was not found within the container");
        }
        if (behavior == UnresolvedTokenBehavior.Replace)
        {
            return TryGetResult.Success(replacementSettings?.UnresolvedTokenReplacement ?? string.Empty);
        }
        return default;
    }

    public static int GetLoopIteration(this ExpanderContext context, ISequenceTokenValueContainer sequence) =>
        context.Commands.HasCommand<LoopBlockCommand>()
            ? LoopBlockCommand.GetCurrentIteration(context, sequence)
            : throw new ExpanderException($"Loop command missing from settings");

    public static object? ApplyValueConverter(this ExpanderContext context, object? containerValue, string token)
    {
        var converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
        if (converter != default) { return converter.Value; }
        throw new MissingValueConverterException($"No matching value converter found for value '{containerValue}' in token '{token}'");
    }
}
