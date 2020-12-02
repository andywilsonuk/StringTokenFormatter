using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StringTokenFormatter {

    public interface ITokenValueContainer {
        bool TryMap(IMatchedToken matchedToken, out object? mapped);
    }

}