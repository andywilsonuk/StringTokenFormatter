using System.Text.RegularExpressions;

namespace StringTokenFormatter.Impl;

public class MapCommand : IExpanderCommand
{
    private const string commandName = "map";
    private const string KeyValuePairsPattern = "([^=,]+)=([^,]*)";

    public void Init(ExpanderContext context)
    {
    }

    public void Evaluate(ExpanderContext context)
    {
        var segment = context.SegmentIterator.Current;
        if (segment is not InterpolatedStringCommandSegment commandSegment || !commandSegment.IsCommand(commandName))
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
            return;
        }

        string tokenValueString = tokenValue?.ToString() ?? string.Empty;

        var matchingPairs = Regex.Matches(segment.Data, KeyValuePairsPattern).Cast<Match>();

        var match = matchingPairs.FirstOrDefault(match => string.Equals(match.Groups[1].Value, tokenValueString, StringComparison.InvariantCultureIgnoreCase));
        if (match == default)
        {
            throw new ExpanderException($"Token {segment.Token} does not have a matching map value for {tokenValue}");
        }

        context.StringBuilder.AppendLiteral(match.Groups[2].Value);
        context.SkipRemainingCommands = true;
        return;
    }

    public void Finished(ExpanderContext context)
    {
    }
}