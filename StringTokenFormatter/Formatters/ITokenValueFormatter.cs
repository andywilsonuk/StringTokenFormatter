using System;

namespace StringTokenFormatter {

    public interface ITokenValueFormatter {
        string? Format(ISegment segment, object? value, string? Padding, string? Format);
    }

}
