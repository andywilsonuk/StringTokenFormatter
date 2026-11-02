namespace StringTokenFormatter.Tests;

public class InterpolatedStringResolverTraceTests
{
    [Fact]
    public void FromContainer_RecordsSegmentProcessingAndTokenLookups()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.LeaveUnresolved,
        };
        InterpolatedStringResolver resolver = new(settings);
        ITokenValueContainer container = resolver.Builder()
            .AddSingle("name", "Ada")
            .CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        string actual = resolver.FromContainer("Hello {name} {missing}", container, trace);

        Assert.Equal("Hello Ada {missing}", actual);

        SegmentProcessingTraceEvent[] segments = trace.Events.OfType<SegmentProcessingTraceEvent>().ToArray();
        Assert.Equal(4, segments.Count(x => x.Status == ExpansionSegmentStatus.Started));
        Assert.Equal(4, segments.Count(x => x.Status == ExpansionSegmentStatus.Completed));
        Assert.Contains(segments, x =>
            x.Status == ExpansionSegmentStatus.Completed
            && x.Segment == "{name}"
            && x.SegmentType == ExpansionSegmentType.Token
            && x.Command == nameof(StandardCommand));

        TokenLookupTraceEvent[] lookups = trace.Events.OfType<TokenLookupTraceEvent>().ToArray();
        Assert.Contains(lookups, x =>
            x.TokenName == "name"
            && x.Purpose == TokenLookupPurpose.TokenValue
            && x.Source == TokenLookupSource.Container
            && x.IsSuccess);
        Assert.Contains(lookups, x =>
            x.TokenName == "missing"
            && x.Source == TokenLookupSource.Container
            && !x.IsSuccess);
        Assert.Contains(lookups, x =>
            x.TokenName == "missing"
            && x.Source == TokenLookupSource.Unresolved
            && !x.IsSuccess);
    }

    [Fact]
    public void FromContainer_RecordsLoopSequenceAndPseudoTokenLookups()
    {
        InterpolatedStringResolver resolver = new(StringTokenFormatterSettings.Default);
        ITokenValueContainer container = resolver.Builder()
            .AddSequence("Items", new[] { new { Value = "A" }, new { Value = "B" } })
            .CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        string actual = resolver.FromContainer("{:loop,Items}{Items.Value}{::loopiteration}{:loopend}", container, trace);

        Assert.Equal("A1B2", actual);
        Assert.Contains(trace.Events.OfType<TokenLookupTraceEvent>(), x =>
            x.TokenName == "Items"
            && x.Purpose == TokenLookupPurpose.Loop
            && x.Source == TokenLookupSource.Container
            && x.IsSuccess);
        Assert.Equal(2, trace.Events.OfType<TokenLookupTraceEvent>().Count(x =>
            x.TokenName == "Items.Value"
            && x.Source == TokenLookupSource.Sequence
            && x.IsSuccess));
        Assert.Equal(2, trace.Events.OfType<TokenLookupTraceEvent>().Count(x =>
            x.TokenName == "::loopiteration"
            && x.Source == TokenLookupSource.PseudoCommand
            && x.IsSuccess));
        Assert.Equal(2, trace.Events.OfType<SegmentProcessingTraceEvent>().Count(x =>
            x.Segment == "{Items.Value}"
            && x.Status == ExpansionSegmentStatus.Completed));
    }

    [Fact]
    public void FromContainer_RecordsFailedSegmentAndRethrows()
    {
        InterpolatedStringResolver resolver = new(StringTokenFormatterSettings.Default);
        ITokenValueContainer container = resolver.Builder().AddSingle("name", "Ada").CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        Assert.Throws<UnresolvedTokenException>(() => resolver.FromContainer("{missing}", container, trace));

        Assert.Contains(trace.Events.OfType<SegmentProcessingTraceEvent>(), x =>
            x.Segment == "{missing}"
            && x.Status == ExpansionSegmentStatus.Failed
            && x.ExceptionType == nameof(UnresolvedTokenException));
        Assert.Contains(trace.Events.OfType<TokenLookupTraceEvent>(), x =>
            x.TokenName == "missing"
            && x.Source == TokenLookupSource.Unresolved
            && !x.IsSuccess);
    }

    [Fact]
    public void FromContainer_RecordsReplacementForUnresolvedTokens()
    {
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            UnresolvedTokenBehavior = UnresolvedTokenBehavior.Replace,
            UnresolvedTokenReplacement = "fallback",
        };
        InterpolatedStringResolver resolver = new(settings);
        ITokenValueContainer container = resolver.Builder().AddSingle("name", "Ada").CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        string actual = resolver.FromContainer("{missing}", container, trace);

        Assert.Equal("fallback", actual);
        Assert.Contains(trace.Events.OfType<TokenLookupTraceEvent>(), x =>
            x.TokenName == "missing"
            && x.Source == TokenLookupSource.Container
            && !x.IsSuccess);
        Assert.Contains(trace.Events.OfType<TokenLookupTraceEvent>(), x =>
            x.TokenName == "missing"
            && x.Source == TokenLookupSource.Replacement
            && x.IsSuccess);
    }

    [Fact]
    public void ToReport_FormatsTraceEventsAndEscapesLineBreaks()
    {
        InterpolatedStringResolver resolver = new(StringTokenFormatterSettings.Default);
        ITokenValueContainer container = resolver.Builder()
            .AddSingle("name", "Ada")
            .CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        resolver.FromContainer("Hello\n{name}", container, trace);

        string report = trace.ToReport();

        Assert.StartsWith("Segment 0 Started [Literal]: \"Hello\\n\"", report);
        Assert.Contains("Segment 0 Completed [Literal, StandardCommand]: \"Hello\\n\"", report);
        Assert.Contains("Lookup TokenValue \"name\" via Container: succeeded", report);
        Assert.EndsWith(Environment.NewLine, report);
    }

    [Fact]
    public void ToReport_EmptyTraceReturnsEmptyString()
    {
        Assert.Equal(string.Empty, new InterpolatedStringResolverTrace().ToReport());
    }

    [Fact]
    public void CustomCommand_CanRecordTextInTraceAndReport()
    {
        TextRecordingCommand command = new();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            Commands = [command, .. StringTokenFormatterSettings.Default.Commands],
        };
        InterpolatedStringResolver resolver = new(settings);
        ITokenValueContainer container = resolver.Builder().AddSingle("name", "Ada").CombinedResult();
        InterpolatedStringResolverTrace trace = new();

        string actual = resolver.FromContainer("{name}", container, trace);

        Assert.Equal("Ada", actual);
        Assert.Contains(trace.Events.OfType<TextTraceEvent>(), x => x.Text == "Custom command initialized");
        Assert.Contains("Text: \"Custom command initialized\"", trace.ToReport());
    }

    [Fact]
    public void CustomCommand_ReceivesNoOpTraceWhenTracingIsNotRequested()
    {
        TextRecordingCommand command = new();
        StringTokenFormatterSettings settings = StringTokenFormatterSettings.Default with
        {
            Commands = [command, .. StringTokenFormatterSettings.Default.Commands],
        };
        InterpolatedStringResolver resolver = new(settings);
        ITokenValueContainer container = resolver.Builder().AddSingle("name", "Ada").CombinedResult();

        string actual = resolver.FromContainer("{name}", container);

        Assert.Equal("Ada", actual);
        Assert.NotNull(command.Trace);
        Assert.Empty(command.Trace.Events);
    }

    private sealed class TextRecordingCommand : IExpanderCommand
    {
        public InterpolatedStringResolverTrace? Trace { get; private set; }

        public void Init(ExpanderContext context)
        {
            Trace = context.Trace;
            context.Trace.Record("Custom command initialized");
        }

        public void Evaluate(ExpanderContext context) { }
        public void Finished(ExpanderContext context) { }
    }
}
