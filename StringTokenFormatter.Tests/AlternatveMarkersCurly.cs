using System;
using System.Collections.Generic;

namespace StringTokenFormatter.Tests
{
    internal class AlternatveMarkersCurly : TokenMarkers
    {
        public string StartToken
        {
            get { return "${"; }
        }

        public string EndToken
        {
            get { return "}"; }
        }

        public string StartTokenEscaped
        {
            get { return "${{"; }
        }

        public string EndTokenEscaped
        {
            get { return "}}"; }
        }

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
