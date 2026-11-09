namespace StringTokenFormatter.Impl;

public sealed class ExpandedStringIterator
{
    internal ExpandedStringIterator(IReadOnlyList<InterpolatedStringSegment> segments)
    {
        Segments = segments;
    }

    public IReadOnlyList<InterpolatedStringSegment> Segments { get; }
    public int CurrentIndex { get; private set; } = -1;

    public InterpolatedStringSegment Current => Segments[CurrentIndex];

    public void JumpToIndex(int index)
    {
        if (index < 0 || index >= Segments.Count) { throw new ExpanderException($"Jump to segment index {index} is out of range, max is {Segments.Count - 1}"); }
        CurrentIndex = index;
    }

    public bool MoveNext()
    {
        CurrentIndex++;
        return CurrentIndex < Segments.Count;
    }
}
