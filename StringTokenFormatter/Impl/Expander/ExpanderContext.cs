namespace StringTokenFormatter.Impl;

public sealed class ExpanderContext
{
    internal ExpanderContext(ExpandedStringIterator iterator, ExpandedStringBuilder stringBuilder, ITokenValueContainer container, IInterpolatedStringSettings settings, ExpanderCommands commands, InterpolatedStringResolverTrace trace)
    {
        SegmentIterator = iterator;
        StringBuilder = stringBuilder;
        Container = container;
        Settings = settings;
        Commands = commands;
        Trace = trace;
    }

    public ExpandedStringIterator SegmentIterator { get; init; }
    public ExpandedStringBuilder StringBuilder { get; init; }
    public ITokenValueContainer Container { get; init; }
    public IInterpolatedStringSettings Settings { get; init; }
    public ExpanderCommands Commands { get; init; }
    public ExpanderDataStore DataStore { get; } = new();
    /// <summary>
    /// Gets the trace for this expansion. Custom commands can call <see cref="InterpolatedStringResolverTrace.Record(string)"/>
    /// to add a text entry, even when tracing was not explicitly requested.
    /// </summary>
    public InterpolatedStringResolverTrace Trace { get; init; }

    public bool SegmentHandled { get; set; } = false;

    /// <summary>
    /// Maps the token directly against the container, recording the lookup in the trace.
    /// Unlike <see cref="ExpanderContextExtensions.TryGetTokenValue"/>, this bypasses pseudo commands,
    /// sequence iteration, and unresolved token behavior.
    /// </summary>
    public TryGetResult TryMapContainerToken(string tokenName) => TryMapContainerToken(tokenName, TokenLookupPurpose.TokenValue);

    internal TryGetResult TryMapContainerToken(string tokenName, TokenLookupPurpose purpose)
    {
        TryGetResult result = Container.TryMap(tokenName);
        Trace.RecordTokenLookup(tokenName, purpose, TokenLookupSource.Container, result.IsSuccess);
        return result;
    }
}
