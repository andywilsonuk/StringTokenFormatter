using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests
{
    internal class AlternatveMarkersRound2 : TokenMarkers
    {
        public string StartToken => "$(";

        public string EndToken => ")";

        public string StartTokenEscaped => "$$(";

        public string EndTokenEscaped => ")";

        public IEqualityComparer<string> TokenNameComparer => StringComparer.InvariantCultureIgnoreCase;
    }
}
