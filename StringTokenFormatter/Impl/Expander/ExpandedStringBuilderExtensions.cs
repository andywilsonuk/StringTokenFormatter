namespace StringTokenFormatter.Impl;

public static class ExpandedStringBuilderExtensions
{
    public static void AppendTokenValue(this ExpandedStringBuilder builder, ExpanderContext context, InterpolatedStringTokenSegment tokenSegment)
    {
        if (!context.TryGetTokenValue(tokenSegment.Token, out object? tokenValue))
        {
            builder.AppendLiteral(tokenSegment.Raw);
            return;
        }
        if (tokenValue == null) { return; }
        try
        {
            builder.AppendFormat(tokenValue, tokenSegment.Alignment, tokenSegment.Format);
        }
        catch(FormatException) when (context.Settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveToken)
        {
            builder.AppendLiteral(tokenSegment.Raw);
        }
        catch(FormatException) when (context.Settings.InvalidFormatBehavior == InvalidFormatBehavior.LeaveUnformatted)
        {
            builder.AppendUnformatted(tokenValue);
        }
        catch(FormatException ex)
        {
            throw new TokenValueFormatException($"Unable to format token {tokenSegment.Raw}", ex);
        }
    }
}