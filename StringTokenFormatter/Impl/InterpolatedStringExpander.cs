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
            if (segment is InterpolatedStringTokenSegment tokenSegment)
            {
                string token = tokenSegment.Token;
                if (token.StartsWith(settings.ConditionStartToken, StringComparison.Ordinal))
                {
                    ConditionHandler(enumerator, container, settings, sb);
                }
                else
                {
                    Evaluate(tokenSegment, container, settings, sb);
                }
            }
            else
            {
                sb.Append(segment.Raw);
            }
        }
        return sb.ToString();
    }

    private static void ConditionHandler(IEnumerator<InterpolatedStringSegment> enumerator, ITokenValueContainer container, IInterpolatedStringSettings settings, StringBuilder sb, bool isAncestorMet = true)
    {
        var conditionSegment = (InterpolatedStringTokenSegment)enumerator.Current;
        string startTokenPrefix = settings.ConditionStartToken;
        string endTokenPrefix = settings.ConditionEndToken;

        string actualToken = conditionSegment.Token.Substring(startTokenPrefix.Length);
        if (!TryGetTokenValue(container, settings, actualToken, out object? tokenValue) || tokenValue is not bool boolValue)
        {
            throw new ConditionTokenException($"Condition for token {actualToken} is not a boolean");
        }
        bool isMet = isAncestorMet && boolValue;

        while (enumerator.MoveNext())
        {
            var segment = enumerator.Current;
            if (segment is InterpolatedStringTokenSegment tokenSegment)
            {
                string token = tokenSegment.Token;
                if (token.StartsWith(endTokenPrefix, StringComparison.Ordinal)) { return; }
                if (token.StartsWith(startTokenPrefix, StringComparison.Ordinal))
                {
                    ConditionHandler(enumerator, container, settings, sb, isMet);
                }
                else if (isMet)
                {
                    Evaluate(tokenSegment, container, settings, sb);
                }
            }
            else if (isMet)
            {
                sb.Append(segment.Raw);
            }
        }
        throw new ConditionTokenException($"Missing {endTokenPrefix} for condition {actualToken}");
    }

    private static void Evaluate(InterpolatedStringTokenSegment tokenSegment, ITokenValueContainer container, IInterpolatedStringSettings settings, StringBuilder sb)
    {
        if (!TryGetTokenValue(container, settings, tokenSegment.Token, out object? tokenValue))
        {
            sb.Append(tokenSegment.Raw);
            return;
        }
        if (tokenValue == null) { return; }
        try
        {
            FormatValue(tokenValue, tokenSegment.Alignment, tokenSegment.Format, settings.FormatProvider, sb);
        }
        catch(FormatException) when (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveToken)
        {
            sb.Append(tokenSegment.Raw);
        }
        catch(FormatException) when (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveUnformatted)
        {
            sb.Append(tokenValue);
        }
        catch(FormatException ex)
        {
            throw new TokenValueFormatException($"Unable to format token {tokenSegment.Raw}", ex);
        }
    }

    private static bool TryGetTokenValue(ITokenValueContainer container, IInterpolatedStringSettings settings, string token, out object? value)
    {
        var containerMatch = container.TryMap(token);
        if (containerMatch.IsSuccess)
        {
            var containerValue = containerMatch.Value;
            var converter = settings.ValueConverters.Select(fn => fn(containerValue, token)).FirstOrDefault(x => x.IsSuccess);
            if (converter != default)
            {
                value = converter.Value;
                return true;
            }
            throw new MissingValueConverterException($"No matching value converter found for token '{token}' with container value {containerValue}");
        }
        else if (settings.UnresolvedTokenBehavior == UnresolvedTokenBehavior.Throw)
        {
            throw new UnresolvedTokenException($"Token '{token}' was not found within the container");
        }
        value = null;
        return false;
    }

    private static void FormatValue(object value, string alignment, string formatString, IFormatProvider formatProvider, StringBuilder sb)
    {
        bool isAlignmentEmpty = alignment == string.Empty;
        bool isFormatStringEmpty = formatString == string.Empty;

        if (isAlignmentEmpty && isFormatStringEmpty) {
            sb.Append(Convert.ToString(value, formatProvider));
            return;
        }
        if (isAlignmentEmpty) { alignment = "0"; }
        if (isFormatStringEmpty) { formatString = "G"; }
        sb.AppendFormat(formatProvider, $"{{0,{alignment}:{formatString}}}", value);
    }
}
