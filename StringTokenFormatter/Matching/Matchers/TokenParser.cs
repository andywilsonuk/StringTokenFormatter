using System;

namespace StringTokenFormatter {
    public static class TokenParser {

        public static RegexTokenParser Regex(ITokenMarkers? TokenMarkers = default) => new(TokenMarkers);

        private static ITokenParser __Default = Regex();
        public static ITokenParser Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

    }

}
