namespace StringTokenFormatter.Impl;

public sealed class LoopBlockCommand : IBlockCommand
{
    internal LoopBlockCommand() {}

    private const string startCommandName = "loop";
    private const string endCommandName = "loopend";

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringBlockSegment blockSegment)
        {
            if (blockSegment.IsCommand(startCommandName))
            {
                Start(context, blockSegment);
                context.SkipRemainingBlockCommands = true;
            }
            else if (blockSegment.IsCommand(endCommandName))
            {
                End(context);
                context.SkipRemainingBlockCommands = true;
            }
        }
    }
    
    public void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int currentSegmentIndex = context.SegmentIterator.CurrentIndex;
        int iterations = GetRequiredIterations(context, blockSegment);
        PushStack(context, new IterationData(currentSegmentIndex, iterations));
    }

    public void End(ExpanderContext context)
    {
         if (!TryGetStack(context, out var stack) || stack!.Count == 0)
        {
            throw new ExpanderException("Loop end command block without start");
        }
        var (currentSegmentIndex, iterations) = stack.Pop();

        if (iterations == 1)
        {
            context.SkipRemainingBlockCommands = true;
            return;
        }

        PushStack(context, new IterationData(currentSegmentIndex, iterations - 1));

        context.SegmentIterator.JumpToIndex(currentSegmentIndex);
        context.SkipRemainingBlockCommands = true;
    }

    private static int GetRequiredIterations(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int iterations;
        if (blockSegment.Data != string.Empty)
        {
            if (!int.TryParse(blockSegment.Data, out int dataResult))
            {
                throw new ExpanderException($"Loop data '{blockSegment.Data}' is not an int");
            }
            iterations = dataResult;
        }
        else
        {
            if (!context.TryGetTokenValue(blockSegment.Token, out object? tokenValue) || tokenValue is not int iterationsFromTokenValue)
            {
                throw new ExpanderException($"Loop token '{blockSegment.Token}' value is not an int");
            }
            iterations = iterationsFromTokenValue;
        }
        if (iterations <= 0) { throw new ExpanderException($"Loop iterations cannot be less than zero"); }
        return iterations;
    }

    public void Finished(ExpanderContext context)
    {
        if (TryGetStack(context, out var stack) && stack!.Count > 0)
        {
            throw new ExpanderException("Missing loop end command block");
        }
    }

    private const string storeBucketName = nameof(LoopBlockCommand);
    private const string nestingStackStoreKey = "NestingStack";
    private void PushStack(ExpanderContext context, IterationData iterationData)
    {
        var stack = context.ValueStore.Get(storeBucketName, nestingStackStoreKey, () => new Stack<IterationData>());
        stack.Push(iterationData);
        context.ValueStore.Set(storeBucketName, nestingStackStoreKey, stack);
    }
    private bool TryGetStack(ExpanderContext context, out Stack<IterationData>? stack)
    {
        stack = context.ValueStore.Exists(storeBucketName, nestingStackStoreKey)
            ? context.ValueStore.Get<Stack<IterationData>>(storeBucketName, nestingStackStoreKey, () => throw new ExpanderException("Cannot get loop stack when it has not been created"))
            : null;
        return stack is not null;
    }

    record struct IterationData(int SegmentIndex, int RemainingIterations);
}