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
        bool isAlignmentEmpty = alignment == string.Empty;
        bool isFormatStringEmpty = formatString == string.Empty;
        if (valueFormatter.TryFormat(value, tokenName, formatString, out string formattedValue))
        {
        }
        else if (isFormatStringEmpty)
        {
            formattedValue = Convert.ToString(value, formatProvider) ?? string.Empty;
        }
        else
        {
            formattedValue = string.Format(formatProvider, $"{{0:{formatString}}}", value);
        }

        int requiredAlignment = isAlignmentEmpty ? 0 : int.Parse(alignment);
        string paddingValue = requiredAlignment > 0 ? formattedValue.PadLeft(requiredAlignment) : formattedValue.PadRight(Math.Abs(requiredAlignment));

        sb.Append(paddingValue);
    }

    public string ExpandedString() => sb.ToString();
}