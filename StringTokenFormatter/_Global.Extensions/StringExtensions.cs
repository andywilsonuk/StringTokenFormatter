namespace StringTokenFormatter; 
public static class StringExtensions {
    public static IInterpolatedString ToInterpolatedString(this string This) => ToInterpolatedString(This, InterpolationSettings.Default.InterpolatedStringParser);

    public static IInterpolatedString ToInterpolatedString(this string This, IInterpolationSettings Settings) => ToInterpolatedString(This, Settings.InterpolatedStringParser);

    public static IInterpolatedString ToInterpolatedString(this string This, IInterpolatedStringParser Parser) {
        var ret = Parser.Parse(This);

        return ret;
    }
}
