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
            if (isAlignmentEmpty) { alignment = "0"; }
            sb.AppendFormat(formatProvider, $"{{0,{alignment}}}", formattedValue);
            return;
        }

        if (isAlignmentEmpty && isFormatStringEmpty) {
            sb.Append(Convert.ToString(value, formatProvider));
            return;
        }
        if (isAlignmentEmpty) { alignment = "0"; }
        if (isFormatStringEmpty) { formatString = "G"; }
        sb.AppendFormat(formatProvider, $"{{0,{alignment}:{formatString}}}", value);
    }

    public string ExpandedString() => sb.ToString();
}