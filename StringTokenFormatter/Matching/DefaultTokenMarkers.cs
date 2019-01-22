using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class DefaultTokenMarkers : TokenMarkers
    {
        public string StartToken { get; } = "{";
        public string EndToken { get; } = "}";
        public string StartTokenEscaped { get; } = "{{";

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.CurrentCultureIgnoreCase; }
        }
    }
}
