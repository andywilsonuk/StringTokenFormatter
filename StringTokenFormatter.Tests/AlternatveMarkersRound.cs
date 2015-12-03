using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter.Tests
{
    internal class AlternatveMarkersRound : TokenMarkers
    {
        public string StartToken
        {
            get { return "$("; }
        }

        public string EndToken
        {
            get { return ")"; }
        }

        public string StartTokenEscaped
        {
            get { return "$(("; }
        }

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
