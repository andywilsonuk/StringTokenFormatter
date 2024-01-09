using System.Text;

namespace StringTokenFormatter.Impl;

public sealed class ExpandedStringBuilder
{
    private readonly StringBuilder sb = new();
    private readonly IFormatProvider formatProvider;

    public ExpandedStringBuilder(IFormatProvider formatProvider)
    {
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

    public void AppendFormat(object value, string alignment, string formatString)
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

    public string ExpandedString() => sb.ToString();
}