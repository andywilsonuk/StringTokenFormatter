using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    public sealed class DollarCurlyTokenMarkers : ITokenMarkers {
        public string StartToken => "${";
        public string EndToken => "}";
        public string StartTokenEscaped => "${{";

        private DollarCurlyTokenMarkers() { }

        public static DollarCurlyTokenMarkers Instance { get; } = new DollarCurlyTokenMarkers();

    }
}
