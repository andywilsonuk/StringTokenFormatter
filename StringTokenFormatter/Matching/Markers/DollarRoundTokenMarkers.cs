namespace StringTokenFormatter {

    public sealed class DollarRoundTokenMarkers : ITokenMarkers {
        public string StartToken => "$(";
        public string EndToken => ")";
        public string StartTokenEscaped => "$((";

        private DollarRoundTokenMarkers() { }

        public static DollarRoundTokenMarkers Instance { get; } = new DollarRoundTokenMarkers();

    }
}
