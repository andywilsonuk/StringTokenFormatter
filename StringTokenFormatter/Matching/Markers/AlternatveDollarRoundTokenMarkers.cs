namespace StringTokenFormatter {

    public sealed class AlternatveDollarRoundTokenMarkers : ITokenMarkers {
        public string StartToken { get; } = "$(";
        public string EndToken { get; } = ")";
        public string StartTokenEscaped { get; } = "$$(";

        private AlternatveDollarRoundTokenMarkers() { }

        public static AlternatveDollarRoundTokenMarkers Instance { get; } = new AlternatveDollarRoundTokenMarkers();

    }

}
