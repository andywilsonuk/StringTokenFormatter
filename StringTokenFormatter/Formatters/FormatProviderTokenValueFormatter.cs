using System;

namespace StringTokenFormatter {
    public class FormatProviderTokenValueFormatter : ITokenValueFormatter {
        private readonly IFormatProvider provider;

        public FormatProviderTokenValueFormatter(IFormatProvider formatProvider) {
            provider = formatProvider;
        }

        public string? Format(ISegment segment, object? value, string? Padding, string? Format) {
            var ret = default(string?);

            if (value is { }) {
                if (string.IsNullOrEmpty(Padding) && string.IsNullOrEmpty(Format)) {
                    ret = value.ToString();
                } else {
                    var padding = string.IsNullOrEmpty(Padding) ? "0" : Padding;
                    var format = $"{{0,{padding}:{Format}}}";
                    ret = string.Format(provider, format, value);
                }

            }

            return ret;
        }
    }
}
