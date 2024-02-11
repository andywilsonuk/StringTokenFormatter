namespace StringTokenFormatter.Impl;

public class ConditionalBlockCommand : IBlockCommand
{
    internal ConditionalBlockCommand() {}
    
    private const string startCommandName  = "if";
    private const string endCommandName = "ifend";

    public void Init(ExpanderContext context)
    {
        SetDisabledCount(context, 0);
        SetNestedCount(context, 0);
    }

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
        int disabledCount = GetDisabledCount(context);
        context.SkipRemainingBlockCommands = disabledCount > 0;
    }

    private static void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        SetNestedCount(context, GetNestedCount(context) + 1);
        int disabledCount = GetDisabledCount(context);
        
        if (disabledCount > 0)
        {
            SetDisabledCount(context, disabledCount + 1);
            return;
        }

        string tokenName = blockSegment.Token;
        bool isNegated = tokenName[0] == '!';
        string actualTokenName = isNegated ? tokenName[1..] : tokenName;

        TryGetResult containerMatch;
        if (context.TryGetSequence(actualTokenName, out var sequence))
        {
            int currentIteration = context.GetLoopIteration(sequence);
            containerMatch = sequence.TryMap(actualTokenName, currentIteration);
        }
        else
        {
            containerMatch = context.Container.TryMap(actualTokenName);
        }
        
        if (!context.ConvertValueIfMatched(containerMatch, actualTokenName, out object? tokenValue) || tokenValue is not bool conditionEnabled)
        {
            throw new ExpanderException($"Conditional token value '{actualTokenName}' is not a boolean");
        }

        if (conditionEnabled != isNegated) { return; }
        SetDisabledCount(context, disabledCount + 1);
    }

    private static void End(ExpanderContext context)
    {
        SetNestedCount(context, GetNestedCount(context) - 1);
        SetDisabledCount(context, GetDisabledCount(context) - 1);
    }
    
    public void Finished(ExpanderContext context)
    {
        int nestedCount = GetNestedCount(context);
        if (nestedCount != 0) { throw new ExpanderException("Mismatch of conditional commands start and end counts"); }
    }

    private const string storeBucketName = nameof(ConditionalBlockCommand);
    private const string disableCountStoreKey = "DisabledCount";
    private const string nestCountStoreKey = "NestedCount";
    private static int GetDisabledCount(ExpanderContext context) => context.DataStore.Get<int>(storeBucketName, disableCountStoreKey);
    private static void SetDisabledCount(ExpanderContext context, int disabledCount) => context.DataStore.Set(storeBucketName, disableCountStoreKey, disabledCount);
    private static int GetNestedCount(ExpanderContext context) => context.DataStore.Get<int>(storeBucketName, nestCountStoreKey);
    private static void SetNestedCount(ExpanderContext context, int nestingCount) => context.DataStore.Set(storeBucketName, nestCountStoreKey, nestingCount);
}