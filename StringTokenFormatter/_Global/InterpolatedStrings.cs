using StringTokenFormatter.Impl;

namespace StringTokenFormatter;

public static class InterpolatedStrings {

    public static IInterpolatedString Create(params IInterpolatedStringSegment[] Segments) => Create(Segments.AsEnumerable());

    public static IInterpolatedString Create(IEnumerable<IInterpolatedStringSegment> Segments) {
        var ret = new InterpolatedStringImpl(Segments);
        return ret;
    }
}
