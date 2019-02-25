using System;
using System.Collections.Generic;

namespace StringTokenFormatter
{
    public class DefaultTokenMarkers : ITokenMarkers
    {
        public string StartToken { get; } = "{";
        public string EndToken { get; } = "}";
        public string StartTokenEscaped { get; } = "{{";
        public IEqualityComparer<string> TokenNameComparer => StringComparer.CurrentCultureIgnoreCase;
    }
}
