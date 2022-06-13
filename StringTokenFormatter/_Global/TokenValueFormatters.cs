using System;

namespace StringTokenFormatter {
    public static class TokenValueFormatters {

        public static ITokenValueFormatter Default { get; }

        public static ITokenValueFormatter InvariantCulture { get; }
        public static ITokenValueFormatter InstalledUICulture { get; }
        public static ITokenValueFormatter CurrentCulture { get; }
        public static ITokenValueFormatter CurrentUICulture { get; } 

        static TokenValueFormatters() {
            InvariantCulture = From(System.Globalization.CultureInfo.InvariantCulture);
            InstalledUICulture = From(System.Globalization.CultureInfo.InstalledUICulture);
            CurrentCulture = From(System.Globalization.CultureInfo.CurrentCulture);
            CurrentUICulture = From(System.Globalization.CultureInfo.CurrentUICulture);

            Default = CurrentUICulture;
        }

        public static ITokenValueFormatter From(IFormatProvider formatProvider) {
            var ret = default(ITokenValueFormatter);
            
            if (formatProvider == System.Globalization.CultureInfo.InvariantCulture) {
                ret = InvariantCulture;
            } else if (formatProvider == System.Globalization.CultureInfo.InstalledUICulture) {
                ret = InstalledUICulture;
            } else if (formatProvider == System.Globalization.CultureInfo.CurrentCulture) {
                ret = CurrentCulture;
            } else if (formatProvider == System.Globalization.CultureInfo.CurrentUICulture) {
                ret = CurrentUICulture;
            } 

            if(ret is null) {
                ret = new Impl.FormatProviderTokenValueFormatterImpl(formatProvider);
            }

            return ret;
        }

    }

}
