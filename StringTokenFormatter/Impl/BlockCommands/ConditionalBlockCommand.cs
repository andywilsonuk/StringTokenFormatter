namespace StringTokenFormatter.Impl;

public class ConditionalBlockCommand : IBlockCommand
{
    internal ConditionalBlockCommand() {}
    
    private const string startCommandName  = "if";
    private const string endCommandName = "ifend";

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

    public void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        SetNestedCount(context, GetNestedCount(context) + 1);
        int disabledCount = GetDisabledCount(context);
        
        if (disabledCount > 0)
        {
            SetDisabledCount(context, disabledCount + 1);
            return;
        }
        
        if (!context.TryGetTokenValue(blockSegment.Token, out object? tokenValue) || tokenValue is not bool conditionEnabled)
        {
            throw new ExpanderException($"Conditional token value '{blockSegment.Token}' is not a boolean");
        }
        if (!conditionEnabled)
        {
            SetDisabledCount(context, disabledCount + 1);
        }
    }

    public void End(ExpanderContext context)
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
    private static int GetDisabledCount(ExpanderContext context) => context.DataStore.Get(storeBucketName, disableCountStoreKey, () => 0);
    private static void SetDisabledCount(ExpanderContext context, int disabledCount) => context.DataStore.Set(storeBucketName, disableCountStoreKey, disabledCount);
    private static int GetNestedCount(ExpanderContext context) => context.DataStore.Get(storeBucketName, nestCountStoreKey, () => 0);
    private static void SetNestedCount(ExpanderContext context, int nestingCount) => context.DataStore.Set(storeBucketName, nestCountStoreKey, nestingCount);
}