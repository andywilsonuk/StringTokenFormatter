namespace StringTokenFormatter.Impl;

internal class FormatProviderTokenValueFormatterImpl : ITokenValueFormatter {
    private readonly IFormatProvider provider;

    public FormatProviderTokenValueFormatterImpl(IFormatProvider formatProvider) {
        provider = formatProvider;
    }

    public string? Format(IInterpolatedStringSegment segment, object? value, string? Padding, string? Format) {
        var ret = default(string?);

        if (value is { }) {
            if (string.IsNullOrEmpty(Padding) && string.IsNullOrEmpty(Format)) {
                ret = value.ToString();
            } else {
                var padding = string.IsNullOrEmpty(Padding) ? "0" : Padding;
                var format = $"{{0,{padding}:{Format}}}";

                try {
                    ret = string.Format(provider, format, value);
                } catch {
                    ret = value.ToString();
                }

            }

        }

        return ret;
    }
}
