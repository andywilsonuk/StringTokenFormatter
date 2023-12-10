using System.Text;

namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        if (interpolatedString == null) { throw new ArgumentNullException(nameof(interpolatedString)); }
        if (container == null) { throw new ArgumentNullException(nameof(container)); }

        var settings = interpolatedString.Settings;
        var sb = new StringBuilder();
        var enumerator = interpolatedString.Segments.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var segment = enumerator.Current;
            if (segment is InterpolatedStringTokenSegment s && s.Token.StartsWith(settings.ConditionStartToken))
            {
                ConditionHandler(enumerator, container, settings, sb);
            }
            else
            {
                Evaluate(segment, container, settings, sb);
            }
        }
        return sb.ToString();
    }

    private static void ConditionHandler(IEnumerator<InterpolatedStringSegment> enumerator, ITokenValueContainer container, IInterpolatedStringSettings settings, StringBuilder sb)
    {
        var conditionSegment = (InterpolatedStringTokenSegment)enumerator.Current;
        var sbNested = new StringBuilder();
        string actualToken = conditionSegment.Token.Substring(settings.ConditionStartToken.Length);
        object? tokenValue = GetTokenValue(container, settings, actualToken, string.Empty);
        if (tokenValue == null || tokenValue is not bool)
        {
            throw new ConditionTokenException($"Condition for token {actualToken} is not a boolean");
        }
        bool isMet = (bool)tokenValue;

        while (enumerator.MoveNext())
        {
            var segment = enumerator.Current;
            if (segment is InterpolatedStringTokenSegment s && s.Token.StartsWith(settings.ConditionStartToken, StringComparison.OrdinalIgnoreCase))
            {
                ConditionHandler(enumerator, container, settings, sbNested);
            }
            else if (segment is InterpolatedStringTokenSegment s2 && s2.Token.StartsWith(settings.ConditionEndToken, StringComparison.OrdinalIgnoreCase))
            {
                if (isMet) { sb.Append(sbNested); }
                return;
            }
            else
            {
                Evaluate(segment, container, settings, sbNested);
            }
        }
        throw new ConditionTokenException($"Missing {settings.ConditionEndToken} for condition {actualToken}");
    }

    private static void Evaluate(InterpolatedStringSegment segment, ITokenValueContainer container, IInterpolatedStringSettings settings, StringBuilder sb)
    {
        var tokenSegment = segment as InterpolatedStringTokenSegment;
        object? tokenValue = null;
        try
        {
            if (tokenSegment == null)
            { 
                sb.Append(segment.Raw);
                return;
            }

            tokenValue = GetTokenValue(container, settings, tokenSegment.Token, segment.Raw);
            FormatValue(tokenValue, tokenSegment.Alignment, tokenSegment.Format, settings.FormatProvider, sb);
        }
        catch(FormatException ex)
        {
            if (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveToken) {
                sb.Append(segment.Raw);
            }
            else if (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveUnformatted)
            {
                if (tokenValue == null) { return; }
                sb.Append(tokenValue);
            }
            else
            {
                throw new TokenValueFormatException($"Unable to format token {segment.Raw}", ex);
            }
        }
    }

    private static object? GetTokenValue(ITokenValueContainer container, IInterpolatedStringSettings settings, string token, string raw)
    {
        var containerMatch = container.TryMap(token);
        if (containerMatch.IsSuccess)
        {
            return ConvertValue(token, containerMatch.Value, settings);
        }
        else if (settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{token}' was not found within the container");
        }
        return raw;
    }

    private static object? ConvertValue(string token, object? value, IInterpolatedStringSettings settings)
    {
        var converter = settings.ValueConverters.Select(fn => fn(value, token)).FirstOrDefault(x => x.IsSuccess);
        return converter == default ? value : converter.Value;
    }

    private static void FormatValue(object? value, string alignment, string formatString, IFormatProvider formatProvider, StringBuilder sb)
    {
        if (value is null) { return; }

        bool isAlignmentEmpty = alignment == string.Empty;
        bool isFormatStringEmpty = formatString == string.Empty;

        if (isAlignmentEmpty && isFormatStringEmpty) {
            sb.AppendFormat(formatProvider, "{0}", value);
            return;
        }
        if (isAlignmentEmpty) { alignment = "0"; }
        if (isFormatStringEmpty) { formatString = "G"; }
        sb.AppendFormat(formatProvider, $"{{0,{alignment}:{formatString}}}", value);
    }
}
