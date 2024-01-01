namespace StringTokenFormatter.Impl;

public static class InterpolatedStringExpander
{
    public static string Expand(InterpolatedString interpolatedString, ITokenValueContainer container)
    {
        Guard.NotNull(interpolatedString, nameof(interpolatedString));
        Guard.NotNull(container, nameof(container));

        var settings = interpolatedString.Settings;
        var builder = new ExpandedStringBuilder(settings.FormatProvider);
        var enumerator = interpolatedString.Segments.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var segment = enumerator.Current;
            if (segment is InterpolatedStringTokenSegment tokenSegment)
            {
                string token = tokenSegment.Token;
                if (token.StartsWith(settings.ConditionStartToken, StringComparison.Ordinal))
                {
                    ConditionHandler(builder, enumerator, container, settings);
                }
                else
                {
                    Evaluate(builder, tokenSegment, container, settings);
                }
            }
            else
            {
                builder.AppendLiteral(segment.Raw);
            }
        }
        return builder.ExpandedString();
    }

    private static void ConditionHandler(ExpandedStringBuilder builder, IEnumerator<InterpolatedStringSegment> enumerator, ITokenValueContainer container, IInterpolatedStringSettings settings)
    {
        var conditionSegment = (InterpolatedStringTokenSegment)enumerator.Current;
        string startTokenPrefix = settings.ConditionStartToken;
        string endTokenPrefix = settings.ConditionEndToken;

        string actualToken = conditionSegment.Token.Substring(startTokenPrefix.Length);
        if (!TryGetTokenValue(container, settings, actualToken, out object? tokenValue) || tokenValue is not bool conditionEnabled)
        {
            throw new ConditionTokenException($"Condition for token {actualToken} is not a boolean");
        }
        if (!conditionEnabled)
        {
            builder.Disable();
        }

        while (enumerator.MoveNext())
        {
            var segment = enumerator.Current;
            if (segment is InterpolatedStringTokenSegment tokenSegment)
            {
                string token = tokenSegment.Token;
                if (token.StartsWith(endTokenPrefix, StringComparison.Ordinal)) {
                    builder.Enable();
                    return;
                }
                if (token.StartsWith(startTokenPrefix, StringComparison.Ordinal))
                {
                    ConditionHandler(builder, enumerator, container, settings);
                }
                else
                {
                    Evaluate(builder, tokenSegment, container, settings);
                }
            }
            else
            {
                builder.AppendLiteral(segment.Raw);
            }
        }
        throw new ConditionTokenException($"Missing {endTokenPrefix} for condition {actualToken}");
    }

    private static void Evaluate(ExpandedStringBuilder builder, InterpolatedStringTokenSegment tokenSegment, ITokenValueContainer container, IInterpolatedStringSettings settings)
    {
        if (builder.IsDisabled) { return; }
        if (!TryGetTokenValue(container, settings, tokenSegment.Token, out object? tokenValue))
        {
            builder.AppendLiteral(tokenSegment.Raw);
            return;
        }
        if (tokenValue == null) { return; }
        try
        {
            builder.AppendFormat(tokenValue, tokenSegment.Alignment, tokenSegment.Format);
        }
        catch(FormatException) when (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveToken)
        {
            builder.AppendLiteral(tokenSegment.Raw);
        }
        catch(FormatException) when (settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveUnformatted)
        {
            builder.AppendUnformatted(tokenValue);
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
}
