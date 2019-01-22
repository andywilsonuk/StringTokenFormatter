using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests
{
    internal class AlternatveMarkersCurly : TokenMarkers
    {
        public string StartToken { get; } = "${";
        public string EndToken { get; } = "}";
        public string StartTokenEscaped { get; } = "${{";

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
