namespace StringTokenFormatter {
    public static class TokenValueContainerExtensions {
        public static string FormatToken(this ITokenValueContainer Container, string Input, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            var Format2 = SegmentedString.Parse(Input, Parser);

            return Container.FormatToken(Format2, Formatter, Converter, Parser);
        }

        public static string FormatToken(this ITokenValueContainer Container, SegmentedString Input, ITokenValueFormatter Formatter = default, ITokenValueConverter Converter = default, ITokenParser Parser = default) {
            var ret = Input.Format(Container, Formatter, Converter);
            return ret;
        }

    }

}
