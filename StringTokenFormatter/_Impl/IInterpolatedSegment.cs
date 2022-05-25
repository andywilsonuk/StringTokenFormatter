using StringTokenFormatter.Impl;

namespace StringTokenFormatter.Impl {

    public interface IInterpolatedStringSegment {
        string Original { get; }

        string? Evaluate(ITokenValueContainer container, ITokenValueConverter converter, ITokenValueFormatter formatter);

    }

}
