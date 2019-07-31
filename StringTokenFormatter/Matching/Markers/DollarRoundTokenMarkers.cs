using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    public sealed class DollarRoundTokenMarkers : ITokenMarkers {
        public string StartToken => "$(";
        public string EndToken => ")";
        public string StartTokenEscaped => "$((";

        public IEqualityComparer<string> TokenNameComparer => StringComparer.CurrentCultureIgnoreCase;

        private DollarRoundTokenMarkers() { }

        public static DollarRoundTokenMarkers Instance { get; } = new DollarRoundTokenMarkers();

    }
}
