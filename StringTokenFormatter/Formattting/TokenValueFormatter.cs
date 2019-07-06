using System;

namespace StringTokenFormatter {
    public static class TokenValueFormatter {
        private static ITokenValueFormatter __Default = CurrentCulture;
        public static ITokenValueFormatter Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static FormatProviderTokenValueFormatter CurrentCulture { get; private set; } = From(System.Globalization.CultureInfo.CurrentCulture);
        public static FormatProviderTokenValueFormatter CurrentUICulture { get; private set; } = From(System.Globalization.CultureInfo.CurrentUICulture);
        public static FormatProviderTokenValueFormatter InstalledUICulture { get; private set; } = From(System.Globalization.CultureInfo.InstalledUICulture);
        public static FormatProviderTokenValueFormatter InvariantCulture { get; private set; } = From(System.Globalization.CultureInfo.InvariantCulture);

        public static FormatProviderTokenValueFormatter From(IFormatProvider formatProvider) {
            return new FormatProviderTokenValueFormatter(formatProvider);
        }



    }

}
