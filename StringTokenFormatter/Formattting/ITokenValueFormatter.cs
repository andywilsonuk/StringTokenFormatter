using System;

namespace StringTokenFormatter {

    public interface ITokenValueFormatter {
        string Format(TokenSegment token, object value);
    }

}
