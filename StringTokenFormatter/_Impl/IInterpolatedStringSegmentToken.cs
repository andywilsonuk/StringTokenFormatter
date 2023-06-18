namespace StringTokenFormatter; 
public interface IInterpolatedStringSegmentToken : IInterpolatedStringSegment, ITokenMatch {
    string? Padding { get; }
    string? Format { get; }
}