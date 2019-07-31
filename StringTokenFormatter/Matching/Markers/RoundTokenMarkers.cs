﻿using System;
using System.Collections.Generic;

namespace StringTokenFormatter {

    public sealed class RoundTokenMarkers : ITokenMarkers {
        public string StartToken => "(";
        public string EndToken => ")";
        public string StartTokenEscaped => "((";

        public IEqualityComparer<string> TokenNameComparer => StringComparer.CurrentCultureIgnoreCase;

        private RoundTokenMarkers() { }

        public static RoundTokenMarkers Instance { get; } = new RoundTokenMarkers();
    }

}
