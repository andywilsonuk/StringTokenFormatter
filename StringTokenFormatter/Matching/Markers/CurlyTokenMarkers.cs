using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    /// <summary>
    /// 
    /// </summary>
    public sealed class CurlyTokenMarkers : ITokenMarkers {
        public string StartToken => "{";
        public string EndToken => "}";
        public string StartTokenEscaped => "{{";

        public IEqualityComparer<string> TokenNameComparer => StringComparer.CurrentCultureIgnoreCase;

        private CurlyTokenMarkers() { }

        public static CurlyTokenMarkers Instance { get; private set; } = new CurlyTokenMarkers();
    }

}
