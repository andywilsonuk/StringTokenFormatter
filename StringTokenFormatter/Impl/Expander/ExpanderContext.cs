namespace StringTokenFormatter.Impl;

public sealed class ExpanderContext
{
    internal ExpanderContext(ExpandedStringIterator iterator, ExpandedStringBuilder stringBuilder, ITokenValueContainer container, IInterpolatedStringSettings settings, IReadOnlyCollection<IBlockCommand> commands)
    {
        SegmentIterator = iterator;
        StringBuilder = stringBuilder;
        Container = container;
        Settings = settings;
        Commands = commands;
    }

    public ExpandedStringBuilder StringBuilder { get; init; }
    public ITokenValueContainer Container { get; init; }
    public IInterpolatedStringSettings Settings { get; init; }
    public IReadOnlyCollection<IBlockCommand> Commands { get; init; }
    public ExpanderValueStore ValueStore { get; } = new();
    public ExpandedStringIterator SegmentIterator { get; init; }

    public bool SkipRemainingBlockCommands { get; set; } = false;
}
