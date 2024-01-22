using System.Text;

namespace StringTokenFormatter.Impl;

public sealed class ExpandedStringBuilder
{
    private readonly StringBuilder sb = new();
    private readonly ExpanderValueFormatter valueFormatter;
    private readonly IFormatProvider formatProvider;

    public ExpandedStringBuilder(ExpanderValueFormatter valueFormatter, IFormatProvider formatProvider)
    {
        this.valueFormatter = Guard.NotNull(valueFormatter, nameof(valueFormatter));
        this.formatProvider = Guard.NotNull(formatProvider, nameof(formatProvider));
    }

    public void AppendLiteral(string literal)
    {
        sb.Append(literal);
    }

    public void AppendUnformatted(object? value)
    {
        sb.Append(value);
    }

    public void AppendFormat(object value, string tokenName, string alignment, string formatString)
    {
        if (!valueFormatter.TryFormat(value, tokenName, formatString, out string formattedValue))
        {
            formattedValue = FormatUsingProvider(value, formatString == string.Empty ? null : formatString);
        }
        string paddingValue = Pad(ParseAlignment(alignment), formattedValue);
        sb.Append(paddingValue);
    }

    private string FormatUsingProvider(object value, string? formatString)
    {
        try
        {
            if (formatProvider.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter)
            {
                string? result = customFormatter.Format(formatString, value, formatProvider);
                if (result != null) { return result; }
            }
            if (value is IFormattable formattable)
            {
                return formattable.ToString(formatString, formatProvider);
            }
            return value.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new FormatException($"IFormatProvider failed to format value '{value}' with formatString '{formatString}'", ex);
        }
    }

    private static int ParseAlignment(string alignment) =>
        alignment == string.Empty ? 0
         : int.TryParse(alignment, out int requestedAlignment) ? requestedAlignment
         : throw new FormatException($"Cannot convert alignment '{alignment}' to int");

    private static string Pad(int requestedAlignment, string formattedValue) => requestedAlignment switch
    {
        > 0 => formattedValue.PadLeft(requestedAlignment),
        < 0 => formattedValue.PadRight(Math.Abs(requestedAlignment)),
        _ => formattedValue,
    };

    public string ExpandedString() => sb.ToString();
}