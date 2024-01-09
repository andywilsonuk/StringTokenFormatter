namespace StringTokenFormatter.Impl;

public sealed class LoopBlockCommand : IBlockCommand
{
    internal LoopBlockCommand() {}

    public string StartCommandName { get; } = "loop";
    public string EndCommandName { get; } = "loopend";

    public void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int nestingCount = GetNestingCount(context.ValueStore);
        SetNestingCount(context.ValueStore, nestingCount + 1);

        int iterations;
        if (blockSegment.Data != string.Empty) 
        {
            if (!int.TryParse(blockSegment.Data, out int dataResult))
            {
                throw new ExpanderException($"Loop data '{blockSegment.Data}' is not a int");
            }
            iterations = dataResult;
        }
        else
        {
            if (!context.TryGetTokenValue(blockSegment.Token, out object? tokenValue) || tokenValue is not int iterationsFromTokenValue)
            {
                throw new ExpanderException($"Loop token '{blockSegment.Token}' value is not a int");
            }
            iterations = iterationsFromTokenValue;
        }
        if (iterations <= 0)
        {
            throw new ExpanderException($"Loop iterations cannot be less than zero");
        }
        SetIterations(context.ValueStore, nestingCount, iterations);
        SetSegments(context.ValueStore, nestingCount);
    }

    public void End(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int nestingCount = GetNestingCount(context.ValueStore);

        var segments = GetSegments(context.ValueStore, nestingCount - 1);
        int iterations = GetIterations(context.ValueStore, nestingCount - 1);

        for (int i = 0; i < iterations - 1; i++)
        {
            foreach (var segment in segments)
            {
                context.CurrentSegment = segment;
                context.EvaluateCurrentSegment();
            }
        }
        segments.Clear();

        SetNestingCount(context.ValueStore, nestingCount - 1);
        context.CurrentSegment = blockSegment;
    }

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.CurrentSegment ?? throw new ExpanderException("Context current segment is expected not to be null");
        if (segment is InterpolatedStringBlockSegment blockSegment && blockSegment.IsCommand(EndCommandName))
        {
            // don't store end of this block
            return;
        }

        int nestingCount = GetNestingCount(context.ValueStore);
        for (int i = 0; i < nestingCount; i++)
        {
            var segmentsList = GetSegments(context.ValueStore, i);
            segmentsList.Add(segment);
        }
    }

    private const string nestingCountStoreKey = "NestingCount";
    private const string memoSegmentsStoreKey = "MemoSegments";
    private const string iterationsStoreKey = "Iterations";
    private int GetNestingCount(ExpanderValueStore store) => store.Get(nameof(LoopBlockCommand), nestingCountStoreKey, () => 0);
    private static void SetNestingCount(ExpanderValueStore store, int nestingCount) => store.Set(nameof(LoopBlockCommand), nestingCountStoreKey, nestingCount);
    private int GetIterations(ExpanderValueStore store, int index) => store.Get(nameof(LoopBlockCommand), $"{iterationsStoreKey}_{index}", () => 0);
    private static void SetIterations(ExpanderValueStore store, int index, int iterations) => store.Set(nameof(LoopBlockCommand), $"{iterationsStoreKey}_{index}", iterations);
    private List<InterpolatedStringSegment> GetSegments(ExpanderValueStore store, int index) =>
        store.Get<List<InterpolatedStringSegment>>(nameof(LoopBlockCommand), $"{memoSegmentsStoreKey}_{index}", () => throw new ExpanderException("Cannot get segments when List has not been created"));
    private void SetSegments(ExpanderValueStore store, int index) =>
        store.Set(nameof(LoopBlockCommand), $"{memoSegmentsStoreKey}_{index}", new List<InterpolatedStringSegment>());
}