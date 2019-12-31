using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public interface ITokenParser {
        SegmentedString Parse(string input);
        string RemoveTokenMarkers(string token);
    }

}
