using System;
using System.Collections.Generic;
namespace AndyWilsonUk.StringTokenFormatter
{
    public interface TokenMarkers
    {
        string StartToken { get; }
        string EndToken { get; }
        string StartTokenEscaped { get; }
        IEqualityComparer<string> TokenNameComparer { get; }
    }
}
