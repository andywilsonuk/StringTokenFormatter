namespace StringTokenFormatter.Impl;

public sealed class LoopBlockCommand : IExpanderCommand, IExpanderPseudoCommands
{
    private const string startCommandName = "loop";
    private const string endCommandName = "loopend";
    private const string currentIterationCommandName = $"{Constants.PseudoPrefix}loopiteration";
    private const string countCommandName = $"{Constants.PseudoPrefix}loopcount";

    public void Init(ExpanderContext context)
    {
        SetData(context, new LoopBlockData());
    }

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringCommandSegment commandSegment)
        {
            if (commandSegment.IsCommandEqual(startCommandName))
            {
                Start(context, commandSegment);
                context.SegmentHandled = true;
                return;
            }
            else if (commandSegment.IsCommandEqual(endCommandName))
            {
                End(context);
                context.SegmentHandled = true;
                return;
            }
        }
        var data = GetData(context);
        if (data.IsLoopActive && data.PeekOrThrow().TotalIterations == 0)
        {
            context.SegmentHandled = true;
            return;
        }
    }

    private static void Start(ExpanderContext context, InterpolatedStringCommandSegment commandSegment)
    {
        int currentSegmentIndex = context.SegmentIterator.CurrentIndex;
        int requestIterations = 0;
        string tokenName = commandSegment.Token;
        if (context.TryGetSequence(tokenName, out var sequence))
        {
            requestIterations = sequence.Count;
        }
        else if (TryGetTokenIterations(context, tokenName, out int? tokenIterations))
        {
            requestIterations = tokenIterations.Value;
        }
        if (TryGetDataIterations(commandSegment.Data, out int? dataIterations))
        {
            if (sequence != null && dataIterations > sequence.Count)
            {
                throw new ExpanderException($"Loop iterations cannot be greater than list count");
            }
            requestIterations = dataIterations.Value;
        }
        if (requestIterations < 0) { throw new ExpanderException($"Loop iterations cannot be less than zero"); }

        var stack = GetData(context).Stack;
        stack.Push(new LoopData
        {
            SegmentIndex = currentSegmentIndex,
            CurrentIteration = 1,
            TotalIterations = requestIterations,
            Sequence = sequence
        });
    }

    private static bool TryGetTokenIterations(ExpanderContext context, string token, [NotNullWhen(true)] out int? iterations)
    {
        var containerMatch = context.Container.TryMap(token);
        if (containerMatch.IsSuccess && containerMatch.Value is not ISequenceTokenValueContainer)
        {
            if (context.ConvertValueIfMatched(containerMatch, token, out object? tokenValue) && tokenValue is int iterationsFromTokenValue)
            {
                iterations = iterationsFromTokenValue;
                return true;
            }
            throw new ExpanderException($"Loop token '{token}' value is not an int");
        }
        iterations = null;
        return false;
    }

    private static bool TryGetDataIterations(string data, [NotNullWhen(true)] out int? iterations)
    {
        if (data == string.Empty)
        {
            iterations = null;
            return false;
        }
        if (int.TryParse(data, out int dataResult))
        {
            iterations = dataResult;
            return true;
        }
        throw new ExpanderException($"Loop data '{data}' is not an int");
    }

    private static void End(ExpanderContext context)
    {
        var data = GetData(context);
        if (!data.IsLoopActive) { throw new ExpanderException("Loop end command without start"); }

        var stack = data.Stack;
        var innermostLoop = stack.Pop();
        var iteration = innermostLoop.CurrentIteration;
        if (iteration >= innermostLoop.TotalIterations) { return; }

        innermostLoop.CurrentIteration += 1;
        stack.Push(innermostLoop);

        context.SegmentIterator.JumpToIndex(innermostLoop.SegmentIndex);
    }

    public void Finished(ExpanderContext context)
    {
        if (GetData(context).IsLoopActive)
        {
            throw new ExpanderException("Missing loop end command");
        }
    }

    public TryGetResult TryMapPseudo(ExpanderContext context, string tokenName)
    {
        if (OrdinalValueHelper.AreEqual(tokenName, currentIterationCommandName))
        {
            var innermostLoop = GetData(context).PeekOrThrow();
            return TryGetResult.Success(innermostLoop.CurrentIteration);
        }
        if (OrdinalValueHelper.AreEqual(tokenName, countCommandName))
        {
            var innermostLoop = GetData(context).PeekOrThrow();
            return TryGetResult.Success(innermostLoop.TotalIterations);
        }
        return default;
    }

    private static void SetData(ExpanderContext context, LoopBlockData data) => context.DataStore.Set(nameof(LoopBlockCommand), data);
    private static LoopBlockData GetData(ExpanderContext context) => context.DataStore.Get<LoopBlockData>(nameof(LoopBlockCommand));

    private class LoopBlockData
    {
        public Stack<LoopData> Stack { get; } = new Stack<LoopData>();

        public LoopData PeekOrThrow() =>
            IsLoopActive ? Stack.Peek() : throw new ExpanderException($"Cannot query iteration when not in a loop");

        public bool IsLoopActive => Stack.Count > 0;
    }

    private class LoopData
    {
        public required int SegmentIndex { get; init; }
        public required int TotalIterations { get; init; }
        public required ISequenceTokenValueContainer? Sequence { get; init; }
        public required int CurrentIteration { get; set; }
    }

    internal LoopBlockCommand() { }

    public static int GetCurrentIteration(ExpanderContext context, ISequenceTokenValueContainer sequence)
    {
        var stack = GetData(context).Stack;
        var data = stack.FirstOrDefault(x => x.Sequence == sequence);
        if (data == default) { throw new ExpanderException($"Cannot get current iteration when not in a loop"); }
        return data.CurrentIteration;
    }
}