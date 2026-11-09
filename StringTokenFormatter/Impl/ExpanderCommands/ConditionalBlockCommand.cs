namespace StringTokenFormatter.Impl;

public sealed class ConditionalBlockCommand : IExpanderCommand
{
    private const string startCommandName = "if";
    private const string endCommandName = "ifend";

    public void Init(ExpanderContext context)
    {
        SetData(context, new ConditionalBlockData());
    }

    public void Evaluate(ExpanderContext context)
    {
        InterpolatedStringSegment segment = context.SegmentIterator.Current;
        if (segment is InterpolatedStringCommandSegment CommandSegment)
        {
            if (CommandSegment.IsCommandEqual(startCommandName))
            {
                Start(context, CommandSegment);
                context.SegmentHandled = true;
                return;
            }
            else if (CommandSegment.IsCommandEqual(endCommandName))
            {
                End(context);
                context.SegmentHandled = true;
                return;
            }
        }
        context.SegmentHandled = GetData(context).DisabledCount > 0;
    }

    private static void Start(ExpanderContext context, InterpolatedStringCommandSegment segment)
    {
        ConditionalBlockData data = GetData(context);
        data.NestedCount += 1;
        SetData(context, data);

        if (data.DisabledCount > 0)
        {
            data.DisabledCount += 1;
            SetData(context, data);
            return;
        }

        string tokenName = segment.Token;
        bool isNegated = tokenName[0] == '!';
        string conditionalTokenName = isNegated ? tokenName[1..] : tokenName;

        TryGetResult tokenValueResult = context.TryGetTokenValue(conditionalTokenName);
        if (!tokenValueResult.IsSuccess)
        {
            context.StringBuilder.AppendLiteral(segment.Raw);
            return;
        }
        object? convertedValue = context.ApplyValueConverter(tokenValueResult.Value, conditionalTokenName);
        bool conditionEnabled = convertedValue switch
        {
            bool booleanValue => booleanValue,
            string stringValue => stringValue.Length > 0,
            null => false,
            _ => throw new ExpanderException($"Conditional token value '{conditionalTokenName}' is not a boolean or string"),
        };

        if (conditionEnabled != isNegated) { return; }
        data.DisabledCount += 1;
        SetData(context, data);
    }

    private static void End(ExpanderContext context)
    {
        ConditionalBlockData data = GetData(context);
        data.NestedCount -= 1;
        data.DisabledCount = Math.Max(0, data.DisabledCount - 1);
        SetData(context, data);
    }

    public void Finished(ExpanderContext context)
    {
        if (GetData(context).NestedCount != 0)
        {
            throw new ExpanderException("Mismatch of conditional commands start and end counts");
        }
    }

    private static void SetData(ExpanderContext context, ConditionalBlockData data) => context.DataStore.Set(nameof(ConditionalBlockCommand), data);
    private static ConditionalBlockData GetData(ExpanderContext context) => context.DataStore.Get<ConditionalBlockData>(nameof(ConditionalBlockCommand));
    private class ConditionalBlockData
    {
        public int DisabledCount { get; set; } = 0;
        public int NestedCount { get; set; } = 0;
    }

    internal ConditionalBlockCommand() { }
}