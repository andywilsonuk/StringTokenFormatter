namespace StringTokenFormatter;

public static partial class FormatTokenExtensions
{

    public static Uri FormatToken<T>(this Uri input, T values) => FormatToken(input, values, InterpolationSettings.Default);

    public static Uri FormatToken<T>(this Uri input, T values, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatToken(this Uri input, object values) => FormatToken(input, values, InterpolationSettings.Default);

    public static Uri FormatToken(this Uri input, object values, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }


    public static Uri FormatToken(this Uri input, string token, object replacementValue) => FormatToken(input, token, replacementValue, InterpolationSettings.Default);

    public static Uri FormatToken(this Uri input, string token, object replacementValue, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, token, replacementValue, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue) => FormatToken(input, token, replacementValue, InterpolationSettings.Default);

    public static Uri FormatToken<T>(this Uri input, string token, T replacementValue, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, token, replacementValue, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatToken<T>(this Uri input, Func<string, ITokenNameComparer, T> values) => FormatToken(input, values, InterpolationSettings.Default);

    public static Uri FormatToken<T>(this Uri input, Func<string, ITokenNameComparer, T> values, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatToken<T>(this Uri input, Func<string, T> values) => FormatToken(input, values, InterpolationSettings.Default);

    public static Uri FormatToken<T>(this Uri input, Func<string, T> values, IInterpolationSettings Settings) {
        var tret = FormatToken(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values) => FormatDictionary(input, values, InterpolationSettings.Default);

    public static Uri FormatDictionary<T>(this Uri input, IEnumerable<KeyValuePair<string, T>> values, IInterpolationSettings Settings) {
        var tret = FormatDictionary(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    public static Uri FormatContainer(this Uri input, ITokenValueContainer values) => FormatContainer(input, values, InterpolationSettings.Default);

    public static Uri FormatContainer(this Uri input, ITokenValueContainer values, IInterpolationSettings Settings) {
        var tret = FormatContainer(input.OriginalString, values, Settings);
        var ret = CreateUri(tret);
        return ret;
    }

    private static Uri CreateUri(string value) {
        var ret = new Uri(value, UriKind.RelativeOrAbsolute);
        return ret;
    }

}
