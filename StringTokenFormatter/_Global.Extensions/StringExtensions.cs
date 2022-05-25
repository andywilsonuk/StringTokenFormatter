using StringTokenFormatter.Impl;

namespace StringTokenFormatter {
    public static class StringExtensions {
        public static IInterpolatedString ToInterpolatedString(this string This) {
            return ToInterpolatedString(This, InterpolationSettings.Default.InterpolatedStringParser);
        }

        public static IInterpolatedString ToInterpolatedString(this string This, IInterpolationSettings Settings) {
            return ToInterpolatedString(This, Settings.InterpolatedStringParser);
        }

        public static IInterpolatedString ToInterpolatedString(this string This, IInterpolatedStringParser Parser) {
            var ret = Parser.Parse(This);

            return ret;
        }
    }
}
