namespace StringTokenFormatter
{
    public static partial class FormatTokenExtensions
    {

        public static string FormatToken<T>(this string input, T values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this string input, T values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            return FormatToken(Template, values, Settings);
        }

        public static string FormatToken(this string input, object values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken(this string input, object values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatToken(Template, values, Settings);
            return ret;
        }


        public static string FormatToken(this string input, string token, object replacementValue) {
            return FormatToken(input, token, replacementValue, InterpolationSettings.Default);
        }

        public static string FormatToken(this string input, string token, object replacementValue, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatToken(Template, token, replacementValue, Settings);
            return ret;
        }

        public static string FormatToken<T>(this string input, string token, T replacementValue) {
            return FormatToken(input, token, replacementValue, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this string input, string token, T replacementValue, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatToken(Template, token, replacementValue, Settings);
            return ret;
        }

        public static string FormatToken<T>(this string input, Func<string, ITokenNameComparer, T> values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this string input, Func<string, ITokenNameComparer, T> values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatToken(Template, values, Settings);
            return ret;
        }

        public static string FormatToken<T>(this string input, Func<string, T> values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this string input, Func<string, T> values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatToken(Template, values, Settings);
            return ret;
        }

        public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values) {
            return FormatDictionary(input, values, InterpolationSettings.Default);
        }

        public static string FormatDictionary<T>(this string input, IEnumerable<KeyValuePair<string, T>> values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatDictionary(Template, values, Settings);
            return ret;
        }

        public static string FormatContainer(this string input, ITokenValueContainer values) {
            return FormatContainer(input, values, InterpolationSettings.Default);
        }

        public static string FormatContainer(this string input, ITokenValueContainer values, IInterpolationSettings Settings) {
            var Template = input.ToInterpolatedString(Settings);
            var ret = FormatContainer(Template, values, Settings);
            return ret;
        }

    }

}
