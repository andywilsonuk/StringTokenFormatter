namespace StringTokenFormatter.Impl;

public static class ExpanderContextExtensions
{
    public static TryGetResult TryGetTokenValue(this ExpanderContext context, string tokenName)
    {
        TryGetResult pseudoMatch = context.Commands.TryMapPseudo(context, tokenName);
        if (OrdinalValueHelper.StartsWith(tokenName, Constants.PseudoPrefix))
        {
            context.Trace.RecordTokenLookup(tokenName, TokenLookupPurpose.TokenValue, TokenLookupSource.PseudoCommand, pseudoMatch.IsSuccess);
        }
        if (pseudoMatch.IsSuccess) { return pseudoMatch; }

        TryGetResult containerMatch = context.TryMapContainerToken(tokenName);
        if (containerMatch.IsSuccess)
        {
            if (containerMatch.Value is not ISequenceTokenValueContainer sequence) { return containerMatch; }

            TryGetResult sequenceMatch = sequence.TryMap(tokenName, GetLoopIteration(context, sequence));
            context.Trace.RecordTokenLookup(tokenName, TokenLookupPurpose.TokenValue, TokenLookupSource.Sequence, sequenceMatch.IsSuccess);
            if (sequenceMatch.IsSuccess) { return sequenceMatch; }

        }
        IUnresolvedTokenReplacementSettings? replacementSettings = context.Settings as IUnresolvedTokenReplacementSettings;
        UnresolvedTokenBehavior defaultBehavior = context.Settings.UnresolvedTokenBehavior;
        UnresolvedTokenBehavior behavior = replacementSettings?.UnresolvedTokenReplacementFormatter(tokenName, defaultBehavior) ?? defaultBehavior;
        Guard.IsDefined(behavior, nameof(IUnresolvedTokenReplacementSettings.UnresolvedTokenReplacementFormatter));
        if (behavior == UnresolvedTokenBehavior.Throw)
        {
            context.Trace.RecordTokenLookup(tokenName, TokenLookupPurpose.TokenValue, TokenLookupSource.Unresolved, false);
            throw new UnresolvedTokenException($"Token '{tokenName}' was not found within the container");
        }
        if (behavior == UnresolvedTokenBehavior.Replace)
        {
            context.Trace.RecordTokenLookup(tokenName, TokenLookupPurpose.TokenValue, TokenLookupSource.Replacement, true);
            return TryGetResult.Success(replacementSettings?.UnresolvedTokenReplacement ?? string.Empty);
        }
        context.Trace.RecordTokenLookup(tokenName, TokenLookupPurpose.TokenValue, TokenLookupSource.Unresolved, false);
        return default;
    }

    public static int GetLoopIteration(this ExpanderContext context, ISequenceTokenValueContainer sequence) =>
        context.Commands.HasCommand<LoopBlockCommand>()
            ? LoopBlockCommand.GetCurrentIteration(context, sequence)
            : throw new ExpanderException($"Loop command missing from settings");

    public static object? ApplyValueConverter(this ExpanderContext context, object? containerValue, string token)
    {
        TryGetResult converter = context.Settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
        if (converter != default) { return converter.Value; }
        throw new MissingValueConverterException($"No matching value converter found for value '{containerValue}' in token '{token}'");
    }
}
