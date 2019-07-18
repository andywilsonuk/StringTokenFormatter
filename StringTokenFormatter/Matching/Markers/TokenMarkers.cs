using System;

namespace StringTokenFormatter {
    public static class TokenMarkers {

        public static CurlyTokenMarkers Curly => CurlyTokenMarkers.Instance;
        public static DollarCurlyTokenMarkers DollarCurly => DollarCurlyTokenMarkers.Instance;

        public static RoundTokenMarkers Round => RoundTokenMarkers.Instance;
        public static DollarRoundTokenMarkers DollarRound => DollarRoundTokenMarkers.Instance;

        private static ITokenMarkers __Default = Curly;
        public static ITokenMarkers Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

    }

}
