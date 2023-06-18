using System.Globalization;

namespace StringTokenFormatter;

public static class TokenValueFormatters {

    public static ITokenValueFormatter Default { get; }

    public static ITokenValueFormatter InvariantCulture { get; }
    public static ITokenValueFormatter InstalledUICulture { get; }
    public static ITokenValueFormatter CurrentCulture { get; }
    public static ITokenValueFormatter CurrentUICulture { get; } 

    static TokenValueFormatters() {
        InvariantCulture = From(CultureInfo.InvariantCulture);
        InstalledUICulture = From(CultureInfo.InstalledUICulture);
        CurrentCulture = From(CultureInfo.CurrentCulture);
        CurrentUICulture = From(CultureInfo.CurrentUICulture);

        Default = CurrentUICulture;
    }

    public static ITokenValueFormatter From(IFormatProvider formatProvider) {
        var ret = default(ITokenValueFormatter);
        
        if (formatProvider == CultureInfo.InvariantCulture) {
            ret = InvariantCulture;
        } else if (formatProvider == CultureInfo.InstalledUICulture) {
            ret = InstalledUICulture;
        } else if (formatProvider == CultureInfo.CurrentCulture) {
            ret = CurrentCulture;
        } else if (formatProvider == CultureInfo.CurrentUICulture) {
            ret = CurrentUICulture;
        } 

        ret ??= new Impl.FormatProviderTokenValueFormatterImpl(formatProvider);

        return ret;
    }

}
