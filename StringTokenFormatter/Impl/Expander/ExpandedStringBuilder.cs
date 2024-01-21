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

        int requestedAlignment = alignment == string.Empty ? 0 : int.Parse(alignment);
        string paddingValue = requestedAlignment > 0 ? formattedValue.PadLeft(requestedAlignment) : formattedValue.PadRight(Math.Abs(requestedAlignment));
        sb.Append(paddingValue);
    }

    private string FormatUsingProvider(object value, string? formatString)
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

    public string ExpandedString() => sb.ToString();
}