using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests
{
    internal class AlternatveMarkersRound2 : ITokenMarkers
    {
        public string StartToken { get; } = "$(";
        public string EndToken { get; } = ")";
        public string StartTokenEscaped { get; } = "$$(";

        public IEqualityComparer<string> TokenNameComparer => StringComparer.InvariantCultureIgnoreCase;
    }
}
