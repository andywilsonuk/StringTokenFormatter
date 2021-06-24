using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace StringTokenFormatter {

    public interface ITokenValueContainerAsync {
        Task<bool> TryMapAsync(IMatchedToken matchedToken, out object? mapped);
    }

}