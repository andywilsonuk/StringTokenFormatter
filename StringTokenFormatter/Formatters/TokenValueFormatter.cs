using System;

namespace StringTokenFormatter {
    public static class TokenValueFormatter {

        private static ITokenValueFormatter __Default;
        public static ITokenValueFormatter Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static FormatProviderTokenValueFormatter CurrentCulture { get; private set; }
        public static FormatProviderTokenValueFormatter CurrentUICulture { get; private set; } 
        public static FormatProviderTokenValueFormatter InstalledUICulture { get; private set; } 
        public static FormatProviderTokenValueFormatter InvariantCulture { get; private set; }
        public static FormatProviderTokenValueFormatter DefaultThreadCurrentCulture { get; private set; }
        public static FormatProviderTokenValueFormatter DefaultThreadCurrentUICulture { get; private set; }

        static TokenValueFormatter() {
            CurrentCulture =  From(System.Globalization.CultureInfo.CurrentCulture);
            CurrentUICulture = From(System.Globalization.CultureInfo.CurrentUICulture);
            InstalledUICulture = From(System.Globalization.CultureInfo.InstalledUICulture);
            InvariantCulture = From(System.Globalization.CultureInfo.InvariantCulture);
            DefaultThreadCurrentCulture = From(System.Globalization.CultureInfo.DefaultThreadCurrentCulture);
            DefaultThreadCurrentUICulture = From(System.Globalization.CultureInfo.DefaultThreadCurrentUICulture);

            Default = InvariantCulture;
        }

        public static FormatProviderTokenValueFormatter From(IFormatProvider formatProvider) {
            return new FormatProviderTokenValueFormatter(formatProvider);
        }

    }

}
