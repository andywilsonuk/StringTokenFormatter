namespace StringTokenFormatter.Impl {

    public interface ITokenValueFormatter {
        string? Format(IInterpolatedStringSegment segment, object? value, string? Padding, string? Format);
    }

}
