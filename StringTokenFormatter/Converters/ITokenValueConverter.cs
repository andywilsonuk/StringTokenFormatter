using System;

namespace StringTokenFormatter {

    /// <summary>
    /// The ITokenValueConverter interface allows you to convert token values from one type to another.  For example, a Lazy&ltString&gt could be converted to a string. 
    /// </summary>
    public interface ITokenValueConverter {
        bool TryConvert(IMatchedToken matchedToken, object? value, out object? mapped);
    }

}
