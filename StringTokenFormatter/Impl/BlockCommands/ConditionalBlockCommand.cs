namespace StringTokenFormatter.Impl;

public class ConditionalBlockCommand : IBlockCommand
{
    internal ConditionalBlockCommand() {}
    
    public string StartCommandName { get; } = "if";
    public string EndCommandName { get; } = "ifend";

    public void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int disabledCount = GetDisabledCount(context.ValueStore);
        
        if (disabledCount > 0)
        {
            SetDisabledCount(context.ValueStore, disabledCount + 1);
            return;
        }
        
        if (!context.TryGetTokenValue(blockSegment.Token, out object? tokenValue) || tokenValue is not bool conditionEnabled)
        {
            throw new ExpanderException($"Conditional token value '{blockSegment.Token}' is not a boolean");
        }
        if (!conditionEnabled)
        {
            SetDisabledCount(context.ValueStore, disabledCount + 1);
        }
    }

    public void End(ExpanderContext context, InterpolatedStringBlockSegment blockSegment)
    {
        int disabledCount = GetDisabledCount(context.ValueStore);
        SetDisabledCount(context.ValueStore, disabledCount - 1);
    }

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.CurrentSegment ?? throw new ExpanderException("Context current segment is expected not to be null");
        if (segment is InterpolatedStringBlockSegment blockSegment && (blockSegment.IsCommand(StartCommandName) || blockSegment.IsCommand(EndCommandName)))
        {
            // allow nested Conditional blocks or end of this block
            return;
        }
        int disabledCount = GetDisabledCount(context.ValueStore);
        context.SkipActiveBlocks = disabledCount > 0;
    }

    private const string disableCountStoreKey = "DisabledCount";
    private static int GetDisabledCount(ExpanderValueStore store) => store.Get(nameof(ConditionalBlockCommand), disableCountStoreKey, () => 0);
    private static void SetDisabledCount(ExpanderValueStore store, int disabledCount) => store.Set(nameof(ConditionalBlockCommand), disableCountStoreKey, disabledCount);
}