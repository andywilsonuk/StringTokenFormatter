using StringTokenFormatter.Impl;

namespace StringTokenFormatter {
    public static class TokenValueContainerExtensions {
        public static string FormatContainer(this ITokenValueContainer container, string input) {
            return input.FormatContainer(container);
        }

        public static string FormatContainer(this ITokenValueContainer container, string input, IInterpolationSettings Settings) {
            return input.FormatContainer(container, Settings);
        }

        public static string FormatContainer(this ITokenValueContainer container, IInterpolatedString input) {
            return input.FormatContainer(container);
        }

        public static string FormatContainer(this ITokenValueContainer container, IInterpolatedString input, IInterpolationSettings Settings) {
            return input.FormatContainer(container, Settings);
        }

    }

}
