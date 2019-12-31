using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    public sealed class CurlyTokenMarkers : ITokenMarkers {
        public string StartToken => "{";
        public string EndToken => "}";
        public string StartTokenEscaped => "{{";

        private CurlyTokenMarkers() { }

        public static CurlyTokenMarkers Instance { get; } = new CurlyTokenMarkers();
    }

}
