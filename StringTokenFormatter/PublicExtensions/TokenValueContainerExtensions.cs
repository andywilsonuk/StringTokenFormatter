namespace StringTokenFormatter {
    public static class TokenValueContainerExtensions {
        public static string FormatToken(this ITokenValueContainer container, string input, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default, ITokenNameComparer nameComparer = default) {
            var Format2 = SegmentedString.Parse(input, parser);

            return container.FormatToken(Format2, formatter, converter, parser, nameComparer);
        }

        public static string FormatToken(this ITokenValueContainer container, SegmentedString input, ITokenValueFormatter formatter = default, ITokenValueConverter converter = default, ITokenParser parser = default, ITokenNameComparer nameComparer = default) {
            var ret = input.Format(container, formatter, converter);
            return ret;
        }

    }

}
