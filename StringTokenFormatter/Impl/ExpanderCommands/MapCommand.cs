namespace StringTokenFormatter.Impl;

public sealed class MapCommand : IExpanderCommand
{
    private const string commandName = "map";
    private const string discardValue = "_";

    public void Evaluate(ExpanderContext context)
    {
        InterpolatedStringSegment segment = context.SegmentIterator.Current;
        if (segment is not InterpolatedStringCommandSegment commandSegment || !commandSegment.IsCommandEqual(commandName))
        {
            return;
        }
        MapValue(context, commandSegment);
        context.SegmentHandled = true;
    }

    private static void MapValue(ExpanderContext context, InterpolatedStringCommandSegment segment)
    {
        string tokenName = segment.Token;
        TryGetResult tokenValueResult = context.TryGetTokenValue(tokenName);
        if (!tokenValueResult.IsSuccess)
        {
            context.StringBuilder.AppendLiteral(segment.Raw);
            return;
        }
        object? convertedValue = context.ApplyValueConverter(tokenValueResult.Value, tokenName);
        string convertedValueString = convertedValue?.ToString() ?? string.Empty;

        if (TryFindMapValue(segment.Data, convertedValueString, out string mapValue))
        {
            context.StringBuilder.AppendLiteral(mapValue);
            return;
        }

        string outputValue = context.Settings.InvalidFormatBehavior switch
        {
            InvalidFormatBehavior.LeaveUnformatted => convertedValueString,
            InvalidFormatBehavior.LeaveToken => segment.Raw,
            _ => throw new ExpanderException($"Token {segment.Token} does not have a matching map value for {convertedValueString}"),
        };
        context.StringBuilder.AppendLiteral(outputValue);
    }

    private static bool TryFindMapValue(string data, string convertedValue, out string mapValue)
    {
        int lastKeyStart = 0;
        int lastKeyLength = 0;
        int lastValueStart = 0;
        int lastValueLength = 0;
        bool hasLastPair = false;
        int fieldStart = 0;

        while (fieldStart < data.Length)
        {
            int fieldEnd = data.IndexOf(',', fieldStart);
            if (fieldEnd < 0)
            {
                fieldEnd = data.Length;
            }

            int keyStart = fieldStart;
            while (keyStart < fieldEnd)
            {
                int equalsIndex = data.IndexOf('=', keyStart, fieldEnd - keyStart);
                if (equalsIndex < 0)
                {
                    break;
                }
                if (equalsIndex == keyStart)
                {
                    keyStart++;
                    continue;
                }

                int keyLength = equalsIndex - keyStart;
                int valueStart = equalsIndex + 1;
                int valueLength = fieldEnd - valueStart;
                lastKeyStart = keyStart;
                lastKeyLength = keyLength;
                lastValueStart = valueStart;
                lastValueLength = valueLength;
                hasLastPair = true;

                if (CultureInfo.InvariantCulture.CompareInfo.Compare(
                    data,
                    keyStart,
                    keyLength,
                    convertedValue,
                    0,
                    convertedValue.Length,
                    CompareOptions.IgnoreCase) == 0)
                {
                    mapValue = data.Substring(valueStart, valueLength);
                    return true;
                }
                break;
            }

            if (fieldEnd == data.Length)
            {
                break;
            }
            fieldStart = fieldEnd + 1;
        }

        if (hasLastPair
            && lastKeyLength == discardValue.Length
            && string.Compare(data, lastKeyStart, discardValue, 0, lastKeyLength, StringComparison.Ordinal) == 0)
        {
            mapValue = data.Substring(lastValueStart, lastValueLength);
            return true;
        }

        mapValue = string.Empty;
        return false;
    }

    internal MapCommand() { }
    public void Init(ExpanderContext context) { }
    public void Finished(ExpanderContext context) { }
}
