using System.Collections.Generic;

namespace StringTokenFormatter {

    public interface ITokenMarkers {
        string StartToken { get; }
        string EndToken { get; }
        string StartTokenEscaped { get; }
        IEqualityComparer<string> TokenNameComparer { get; }
    }

}
