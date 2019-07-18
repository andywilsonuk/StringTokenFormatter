using System;

namespace StringTokenFormatter {
    public class FormatProviderTokenValueFormatter : ITokenValueFormatter {
        private readonly IFormatProvider provider;

        public FormatProviderTokenValueFormatter(IFormatProvider formatProvider) {
            provider = formatProvider;
        }

        public string Format(TokenSegment token, object value) {
            var ret = default(string);

            if (value != null) {
                if (string.IsNullOrEmpty(token.Padding) && string.IsNullOrEmpty(token.Format)) {
                    ret = value.ToString();
                } else {
                    var padding = string.IsNullOrEmpty(token.Padding) ? "0" : token.Padding;
                    var format = $"{{0,{padding}:{token.Format}}}";
                    ret = string.Format(provider, format, value);
                }

            }

            return ret;
        }
    }
}
