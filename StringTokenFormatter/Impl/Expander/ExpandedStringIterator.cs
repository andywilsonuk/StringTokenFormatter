namespace StringTokenFormatter.Impl;

public sealed class ExpandedStringIterator
{
    internal ExpandedStringIterator(IReadOnlyCollection<InterpolatedStringSegment> segments)
    {
        Segments = segments.ToArray();
    }

    public InterpolatedStringSegment[] Segments { get; }
    public int CurrentIndex { get; private set; } = -1;

    public InterpolatedStringSegment Current => Segments[CurrentIndex];

    public void JumpToSegment(int index)
    {
        if (index < 0 || index >= Segments.Length) { throw new ExpanderException($"Jump to segment index {index} is out of range, max is {Segments.Length - 1}"); }
        CurrentIndex = index;
    }

    public bool MoveNext()
    {
        CurrentIndex++;
        return CurrentIndex < Segments.Length;
    }
}
