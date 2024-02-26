using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public sealed partial class MapCommand : IExpanderCommand
{
    private const string commandName = "map";
    private const string keyValuePairsPattern = "([^=,]+)=([^,]*)";
    private const string discardValue = "_";

#if NET8_0_OR_GREATER
    [GeneratedRegex(keyValuePairsPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex GetKeyValuePairsRegex();
# else
    private static readonly Regex keyValuePairsRegex = new(keyValuePairsPattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
    private static Regex GetKeyValuePairsRegex() => keyValuePairsRegex;
#endif

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is not InterpolatedStringCommandSegment commandSegment || !commandSegment.IsCommandEqual(commandName))
        {
            return;
        }
        MapValue(context, commandSegment);
    }

    private static void MapValue(ExpanderContext context, InterpolatedStringCommandSegment segment)
    {
        var tokenValueResult = context.TryMap(segment.Token);
        if (!context.ConvertValueIfMatched(tokenValueResult, segment.Token, out object? tokenValue))
        {
            context.StringBuilder.AppendLiteral(segment.Raw);
            context.SegmentHandled = true;
            return;
        }
        string tokenValueString = tokenValue?.ToString() ?? string.Empty;

        var matchingPairs = GetKeyValuePairsRegex().Matches(segment.Data);
        for (int i = 0; i < matchingPairs.Count; i++)
        {
            var match = matchingPairs[i];
            string matchValue = match.Groups[1].Value;
            if (StringComparer.InvariantCultureIgnoreCase.Equals(matchValue, tokenValueString)
            || (i == matchingPairs.Count - 1 && OrdinalValueHelper.AreEqual(matchValue, discardValue)))
            {
                string mappedValue = match.Groups[2].Value;
                context.StringBuilder.AppendLiteral(mappedValue);
                context.SegmentHandled = true;
                return;
            }
        }

        string outputValue = context.Settings.InvalidFormatBehavior switch
        {
            InvalidFormatBehavior.LeaveUnformatted => tokenValueString,
            InvalidFormatBehavior.LeaveToken => segment.Raw,
            _ => throw new ExpanderException($"Token {segment.Token} does not have a matching map value for {tokenValue}"),
        };
        context.StringBuilder.AppendLiteral(outputValue);
        context.SegmentHandled = true;
    }

    internal MapCommand() { }
    public void Init(ExpanderContext context) { }
    public void Finished(ExpanderContext context) { }
}