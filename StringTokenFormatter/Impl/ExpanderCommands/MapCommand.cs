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
        context.SegmentHandled = true;
    }

    private static void MapValue(ExpanderContext context, InterpolatedStringCommandSegment segment)
    {
        string tokenName = segment.Token;
        var tokenValueResult = context.TryGetTokenValue(tokenName);
        if (!tokenValueResult.IsSuccess)
        {
            context.StringBuilder.AppendLiteral(segment.Raw);
            return;
        }
        var convertedValue = context.ApplyValueConverter(tokenValueResult.Value, tokenName);
        string convertedValueString = convertedValue?.ToString() ?? string.Empty;

        static (string TokenValue, string MapValue) groupResult(Match m) => (TokenValue: m.Groups[1].Value, MapValue: m.Groups[2].Value);
        var matches = GetKeyValuePairsRegex().Matches(segment.Data);
        var match = matches.Cast<Match>().Select(groupResult).FirstOrDefault(m => StringComparer.InvariantCultureIgnoreCase.Equals(m.TokenValue, convertedValueString));
        if (match == default)
        {
            var last = groupResult(matches[^1]);
            if (OrdinalValueHelper.AreEqual(last.TokenValue, discardValue))
            {
                match = last;
            }
        }
        if (match != default)
        {
            context.StringBuilder.AppendLiteral(match.MapValue);
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

    internal MapCommand() { }
    public void Init(ExpanderContext context) { }
    public void Finished(ExpanderContext context) { }
}