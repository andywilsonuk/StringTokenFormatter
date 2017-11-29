using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class DefaultTokenMarkers : TokenMarkers
    {
        private const string startToken = "{";
        private const string endToken = "}";
        private const string escapedStartToken = startToken + startToken;
        private const string escapedEndToken = endToken + endToken;

        public string StartToken => startToken;
        public string EndToken => endToken;
        public string StartTokenEscaped => escapedStartToken;
        public string EndTokenEscaped => escapedEndToken;

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
