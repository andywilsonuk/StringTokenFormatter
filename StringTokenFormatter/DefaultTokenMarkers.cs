using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class DefaultTokenMarkers : TokenMarkers
    {
        public string StartToken
        {
            get { return "{"; }
        }

        public string EndToken
        {
            get { return "}"; }
        }

        public string StartTokenEscaped
        {
            get { return StartToken + StartToken; }
        }

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
