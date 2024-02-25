namespace StringTokenFormatter.Impl;

public sealed class LoopBlockCommand : IExpanderCommand
{
    internal LoopBlockCommand() { }

    private const string startCommandName = "loop";
    private const string endCommandName = "loopend";
    private const string currentIterationCommandName = "::loopiteration";
    private const string countCommandName = "::loopcount";

    public void Init(ExpanderContext context)
    {
        CreateStack(context);
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
        var stack = GetStack(context);
        if (stack.Count > 0 && stack.Peek().TotalIterations == 0)
        {
            context.SegmentHandled = true;
            return;
        }
        if (segment is InterpolatedStringTokenSegment tokenSegment)
        {
            if (EvaluateTokenSegment(context, stack, tokenSegment))
            {
                context.SegmentHandled = true;
                return;
            }
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

        PushStack(context, new LoopData(currentSegmentIndex, 1, requestIterations, sequence));
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

    private static bool EvaluateTokenSegment(ExpanderContext context, Stack<LoopData> stack, InterpolatedStringTokenSegment tokenSegment)
    {
        string tokenName = tokenSegment.Token;
        bool isCurrentIterationPseudo = tokenSegment.IsPseudoEqual(currentIterationCommandName);
        bool isTotalIterationsPseudo = tokenSegment.IsPseudoEqual(countCommandName);
        if (isCurrentIterationPseudo || isTotalIterationsPseudo)
        {
            if (stack.Count == 0)
            {
                throw new ExpanderException($"No current loop to get {currentIterationCommandName}");
            }
            var stackPeek = stack.Peek();
            int value = isCurrentIterationPseudo ? stackPeek.CurrentIteration : stackPeek.TotalIterations;
            context.StringBuilder.AppendTokenValue(context, tokenSegment, value);
            return true;
        }
        if (context.TryGetSequence(tokenName, out var sequence))
        {
            int currentIteration = GetCurrentIteration(context, sequence);
            var result = sequence.TryMap(tokenName, currentIteration);
            if (context.ConvertValueIfMatched(result, tokenName, out object? convertValue))
            {
                context.StringBuilder.AppendTokenValue(context, tokenSegment, convertValue);
            }
            else
            {
                context.StringBuilder.AppendLiteral(tokenSegment.Raw);
            }
            return true;
        }
        return false;
    }

    private static void End(ExpanderContext context)
    {
        var stack = GetStack(context);
        if (stack.Count == 0)
        {
            throw new ExpanderException("Loop end command without start");
        }

        var data = stack.Pop();
        var iteration = data.CurrentIteration;
        if (iteration >= data.TotalIterations) { return; }

        PushStack(context, data with { CurrentIteration = iteration + 1 });
        context.SegmentIterator.JumpToIndex(data.SegmentIndex);
    }

    public void Finished(ExpanderContext context)
    {
        if (GetStack(context).Count > 0)
        {
            throw new ExpanderException("Missing loop end command");
        }
    }

    public TryGetResult TryMapPseudo(ExpanderContext context, string tokenName)
    {
        if (OrdinalValueHelper.AreEqual(tokenName, currentIterationCommandName))
        {
            var stack = GetStack(context);
            if (stack.Count == 0)
            {
                throw new ExpanderException($"No current loop to get {currentIterationCommandName}");
            }
            var stackPeek = stack.Peek();
            return TryGetResult.Success(stackPeek.CurrentIteration);
        }
        if (OrdinalValueHelper.AreEqual(tokenName, countCommandName))
        {
            var stack = GetStack(context);
            if (stack.Count == 0)
            {
                throw new ExpanderException($"No current loop to get {currentIterationCommandName}");
            }
            var stackPeek = stack.Peek();
            return TryGetResult.Success(stackPeek.TotalIterations);
        }
        return default;
    }

    private const string storeBucketName = nameof(LoopBlockCommand);
    private const string nestingStackStoreKey = "NestingStack";
    private static void CreateStack(ExpanderContext context) => context.DataStore.Set(storeBucketName, nestingStackStoreKey, new Stack<LoopData>());
    private static void PushStack(ExpanderContext context, LoopData iterationData) => GetStack(context).Push(iterationData);
    private static Stack<LoopData> GetStack(ExpanderContext context) => context.DataStore.Get<Stack<LoopData>>(storeBucketName, nestingStackStoreKey);

    public static int GetCurrentIteration(ExpanderContext context, ISequenceTokenValueContainer sequence)
    {
        var stack = GetStack(context);
        var data = stack.FirstOrDefault(x => x.Sequence == sequence);
        if (data == default) { throw new ExpanderException($"Cannot get current iteration when not in a loop"); }
        return data.CurrentIteration;
    }

    record struct LoopData(int SegmentIndex, int CurrentIteration, int TotalIterations, ISequenceTokenValueContainer? Sequence);
}