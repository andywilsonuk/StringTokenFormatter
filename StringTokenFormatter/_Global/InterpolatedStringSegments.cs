using StringTokenFormatter.Impl;

namespace StringTokenFormatter {
    public static class InterpolatedStringSegments {
        public static IInterpolatedStringSegmentLiteral FromLiteral(string Original) {
            return new InterpolatedStringSegmentLiteralImpl(Original);
        }

        public static IInterpolatedStringSegmentToken FromToken(string Token, string? Original = default, string? Padding = default, string? Format = default) {
            return new InterpolatedStringSegmentTokenImpl(Original ?? string.Empty, Token, Padding, Format);
        }

    }

}
