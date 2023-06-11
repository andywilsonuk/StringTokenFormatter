namespace StringTokenFormatter
{

    public static partial class FormatTokenExtensions {

        public static string FormatToken<T>(this IInterpolatedString input, T values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this IInterpolatedString input, T values, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromObject(values);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatToken(this IInterpolatedString input, object values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken(this IInterpolatedString input, object values, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromObject(values);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }


        public static string FormatToken(this IInterpolatedString input, string token, object replacementValue) {
            return FormatToken(input, token, replacementValue, InterpolationSettings.Default);
        }

        public static string FormatToken(this IInterpolatedString input, string token, object replacementValue, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromValue(token, replacementValue);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatToken<T>(this IInterpolatedString input, string token, T replacementValue) {
            return FormatToken(input, token, replacementValue, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this IInterpolatedString input, string token, T replacementValue, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromValue(token, replacementValue);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatToken<T>(this IInterpolatedString input, Func<string, ITokenNameComparer, T> values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this IInterpolatedString input, Func<string, ITokenNameComparer, T> values, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromFunc(values);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatToken<T>(this IInterpolatedString input, Func<string, T> values) {
            return FormatToken(input, values, InterpolationSettings.Default);
        }

        public static string FormatToken<T>(this IInterpolatedString input, Func<string, T> values, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromFunc(values);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatDictionary<T>(this IInterpolatedString input, IEnumerable<KeyValuePair<string, T>> values) {
            return FormatDictionary(input, values, InterpolationSettings.Default);
        }

        public static string FormatDictionary<T>(this IInterpolatedString input, IEnumerable<KeyValuePair<string, T>> values, IInterpolationSettings Settings) {
            var Container = Settings.TokenValueContainerFactory.FromDictionary(values);
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

        public static string FormatContainer(this IInterpolatedString input, ITokenValueContainer values) {
            return FormatContainer(input, values, InterpolationSettings.Default);
        }

        public static string FormatContainer(this IInterpolatedString input, ITokenValueContainer values, IInterpolationSettings Settings) {
            var Container = values;
            var ret = input.FormatContainer(Container, Settings.TokenValueConverter, Settings.TokenValueFormatter);
            return ret;
        }

    }

}
