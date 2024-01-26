namespace StringTokenFormatter.Impl;

public sealed class LoopBlockCommand : IBlockCommand
{
    internal LoopBlockCommand() {}

    private const string startCommandName = "loop";
    private const string endCommandName = "loopend";
    private const string currentIterationCommandName = "::loopiteration";

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringBlockSegment blockSegment)
        {
            if (blockSegment.IsCommand(startCommandName))
            {
                Start(context, blockSegment);
                context.SkipRemainingBlockCommands = true;
                return;
            }
            else if (blockSegment.IsCommand(endCommandName))
            {
                End(context);
                context.SkipRemainingBlockCommands = true;
                return;
            }
        }
        bool hasStack = TryGetStack(context, out var stack) && stack!.Count > 0;
     
        if (hasStack && stack!.Peek().TotalIterations == 0)
        {
            context.SkipRemainingBlockCommands = true;
            return;
        }

        if (segment is InterpolatedStringTokenSegment tokenSegment)
        {
            string tokenName = tokenSegment.Token;
            if (context.Settings.NameComparer.Equals(currentIterationCommandName, tokenName))
            {
                context.StringBuilder.AppendTokenValue(context, tokenSegment, stack!.Peek().CurrentIteration);
                context.SkipRemainingBlockCommands = true;
                return;
            }
            bool isList = TryGetTokenList(context, tokenName, out var sequence);
            if (isList)
            {
                var data = (stack?.FirstOrDefault(x => x.Sequence == sequence)) ?? throw new ExpanderException($"No current loop for {tokenName}");
                var result = sequence!.TryMapForIndex(tokenName, data.CurrentIteration - 1);
                if (context.ConvertOnSuccess(result, tokenName, out object? convertValue))
                {
                    context.StringBuilder.AppendTokenValue(context, tokenSegment, convertValue);
                }
                else
                {
                    context.StringBuilder.AppendLiteral(tokenSegment.Raw);
                }
                context.SkipRemainingBlockCommands = true;
            }
        }
    }
    
    private static void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int currentSegmentIndex = context.SegmentIterator.CurrentIndex;
        int requestIterations = 0;
        string tokenName = blockSegment.Token;
        if (TryGetTokenList(context, tokenName, out var sequence))
        {
            requestIterations = sequence!.Count;
        }
        else if (TryGetTokenIterations(context, tokenName, out int? tokenIterations))
        {
            requestIterations = tokenIterations!.Value;
        }

        if (TryGetDataIterations(blockSegment.Data, out int? dataIterations))
        {
            if (sequence != null && dataIterations > sequence.Count)
            {
                throw new ExpanderException($"Loop iterations cannot be greater than list count");
            }
            requestIterations = dataIterations!.Value;
        }

        if (requestIterations < 0) { throw new ExpanderException($"Loop iterations cannot be less than zero"); }

        PushStack(context, new LoopData(currentSegmentIndex, 1, requestIterations, sequence));
    }

    private static bool TryGetTokenList(ExpanderContext context, string token, out ISequenceTokenValueContainer? list)
    {
        var containerMatch = context.Container.TryMap(token);
        list = containerMatch.IsSuccess && containerMatch.Value is ISequenceTokenValueContainer listValue ? listValue : null;
        return list != null;
    }

    private static bool TryGetTokenIterations(ExpanderContext context, string token, out int? iterations)
    {
        var containerMatch = context.Container.TryMap(token);
        if (containerMatch.IsSuccess && containerMatch.Value is not ISequenceTokenValueContainer)
        {
            if (context.ConvertOnSuccess(containerMatch, token, out object? tokenValue) && tokenValue is int iterationsFromTokenValue)
            {
                iterations = iterationsFromTokenValue;
                return true;
            }
            throw new ExpanderException($"Loop token '{token}' value is not an int");
        }
        iterations = null;
        return false;
    }

    private static bool TryGetDataIterations(string data, out int? iterations)
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
        if (!TryGetStack(context, out var stack) || stack!.Count == 0)
        {
            throw new ExpanderException("Loop end block command without start");
        }

        var data = stack.Pop();
        var iteration = data.CurrentIteration;
        if (iteration >= data.TotalIterations) { return; }

        PushStack(context, data with { CurrentIteration = iteration + 1 });
        context.SegmentIterator.JumpToIndex(data.SegmentIndex);
    }

    public void Finished(ExpanderContext context)
    {
        if (TryGetStack(context, out var stack) && stack!.Count > 0)
        {
            throw new ExpanderException("Missing loop end block command");
        }
    }

    private const string storeBucketName = startCommandName;
    private const string nestingStackStoreKey = "NestingStack";
    private static void PushStack(ExpanderContext context, LoopData iterationData)
    {
        var stack = context.DataStore.Get(storeBucketName, nestingStackStoreKey, () => new Stack<LoopData>());
        stack.Push(iterationData);
        context.DataStore.Set(storeBucketName, nestingStackStoreKey, stack);
    }
    private static bool TryGetStack(ExpanderContext context, out Stack<LoopData>? stack)
    {
        stack = context.DataStore.Exists(storeBucketName, nestingStackStoreKey)
            ? context.DataStore.Get<Stack<LoopData>>(storeBucketName, nestingStackStoreKey, () => throw new ExpanderException("Cannot get loop stack when it has not been created"))
            : null;
        return stack is not null;
    }

    record struct LoopData(int SegmentIndex, int CurrentIteration, int TotalIterations, ISequenceTokenValueContainer? Sequence);
}