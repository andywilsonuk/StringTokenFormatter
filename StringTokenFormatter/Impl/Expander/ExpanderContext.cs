namespace StringTokenFormatter.Impl;

public sealed class ExpanderContext
{
    internal ExpanderContext(ExpandedStringIterator iterator, ExpandedStringBuilder stringBuilder, ITokenValueContainer container, IInterpolatedStringSettings settings, ExpanderCommands commands)
    {
        SegmentIterator = iterator;
        StringBuilder = stringBuilder;
        Container = container;
        Settings = settings;
        Commands = commands;
    }

    public ExpandedStringIterator SegmentIterator { get; init; }
    public ExpandedStringBuilder StringBuilder { get; init; }
    public ITokenValueContainer Container { get; init; }
    public IInterpolatedStringSettings Settings { get; init; }
    public ExpanderCommands Commands { get; init; }
    public ExpanderDataStore DataStore { get; } = new();

    public bool SegmentHandled { get; set; } = false;
}
