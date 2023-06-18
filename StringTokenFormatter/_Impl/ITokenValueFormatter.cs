namespace StringTokenFormatter; 

public interface ITokenValueFormatter {
    string? Format(IInterpolatedStringSegment segment, object? value, string? Padding, string? Format);
}
