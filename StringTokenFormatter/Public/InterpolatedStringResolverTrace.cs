using System.Collections.ObjectModel;

namespace StringTokenFormatter;

public abstract record InterpolatedStringResolverTraceEvent
{
    internal InterpolatedStringResolverTraceEvent() { }
}

public enum ExpansionSegmentType
{
    Literal,
    Token,
    Command,
}

public enum ExpansionSegmentStatus
{
    Started,
    Completed,
    Failed,
}

public enum TokenLookupPurpose
{
    TokenValue,
    Loop,
}

public enum TokenLookupSource
{
    PseudoCommand,
    Container,
    Sequence,
    Replacement,
    Unresolved,
}

public sealed record SegmentProcessingTraceEvent(int SegmentIndex, string Segment, ExpansionSegmentType SegmentType, ExpansionSegmentStatus Status, string? Command = null, string? ExceptionType = null) : InterpolatedStringResolverTraceEvent;

public sealed record TokenLookupTraceEvent(string TokenName, TokenLookupPurpose Purpose, TokenLookupSource Source, bool IsSuccess) : InterpolatedStringResolverTraceEvent;

public sealed record TextTraceEvent(string Text) : InterpolatedStringResolverTraceEvent;

/// <summary>
/// Collects segment-processing and token-lookup events from an interpolated string resolver expansion.
/// Use a separate instance for each concurrent expansion.
/// </summary>
public sealed class InterpolatedStringResolverTrace
{
    internal static InterpolatedStringResolverTrace NoOp { get; } = new(true);

    private readonly List<InterpolatedStringResolverTraceEvent> events = [];
    private readonly ReadOnlyCollection<InterpolatedStringResolverTraceEvent> readOnlyEvents;
    private readonly bool isNoOp;

    public InterpolatedStringResolverTrace()
        : this(false)
    {
    }

    private InterpolatedStringResolverTrace(bool isNoOp)
    {
        this.isNoOp = isNoOp;
        readOnlyEvents = events.AsReadOnly();
    }

    public IReadOnlyList<InterpolatedStringResolverTraceEvent> Events => readOnlyEvents;

    /// <summary>
    /// Adds a custom text entry to the trace.
    /// </summary>
    public void Record(string text)
    {
        Guard.NotNull(text, nameof(text));
        if (isNoOp) { return; }
        events.Add(new TextTraceEvent(text));
    }

    /// <summary>
    /// Removes all recorded events.
    /// </summary>
    public void Clear()
    {
        if (isNoOp) { return; }
        events.Clear();
    }

    internal void RecordTokenLookup(string tokenName, TokenLookupPurpose purpose, TokenLookupSource source, bool isSuccess)
    {
        if (isNoOp) { return; }
        events.Add(new TokenLookupTraceEvent(tokenName, purpose, source, isSuccess));
    }

    internal void RecordSegmentStarted(int segmentIndex, InterpolatedStringSegment segment)
    {
        if (isNoOp) { return; }
        events.Add(new SegmentProcessingTraceEvent(segmentIndex, segment.Raw, GetSegmentType(segment), ExpansionSegmentStatus.Started));
    }

    internal void RecordSegmentCompleted(int segmentIndex, InterpolatedStringSegment segment, IExpanderCommand? command)
    {
        if (isNoOp) { return; }
        events.Add(new SegmentProcessingTraceEvent(segmentIndex, segment.Raw, GetSegmentType(segment), ExpansionSegmentStatus.Completed, command?.GetType().Name));
    }

    internal void RecordSegmentFailed(int segmentIndex, InterpolatedStringSegment segment, IExpanderCommand? command, string exceptionType)
    {
        if (isNoOp) { return; }
        events.Add(new SegmentProcessingTraceEvent(segmentIndex, segment.Raw, GetSegmentType(segment), ExpansionSegmentStatus.Failed, command?.GetType().Name, exceptionType));
    }

    private static ExpansionSegmentType GetSegmentType(InterpolatedStringSegment segment) => segment switch
    {
        InterpolatedStringCommandSegment => ExpansionSegmentType.Command,
        InterpolatedStringTokenSegment => ExpansionSegmentType.Token,
        _ => ExpansionSegmentType.Literal,
    };
}
