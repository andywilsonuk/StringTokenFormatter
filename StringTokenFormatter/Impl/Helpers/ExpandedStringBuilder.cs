using System.Text;

namespace StringTokenFormatter.Impl;

public class ExpandedStringBuilder
{
    private readonly StringBuilder sb = new();
    private readonly IFormatProvider formatProvider;
    private int disabledCount = 0;

    public ExpandedStringBuilder(IFormatProvider formatProvider)
    {
        this.formatProvider = Guard.NotNull(formatProvider, nameof(formatProvider));
    }

    public void AppendLiteral(string literal)
    {
        if (IsDisabled) { return; }
        sb.Append(literal);
    }

    public void AppendUnformatted(object? value)
    {
        if (IsDisabled) { return; }
        sb.Append(value);
    }

    public void AppendFormat(object value, string alignment, string formatString)
    {
        if (IsDisabled) { return; }
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

    public void Disable() => disabledCount++;
    public void Enable() => disabledCount--;
    public bool IsDisabled => disabledCount > 0;
}