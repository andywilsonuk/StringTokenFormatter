namespace StringTokenFormatter.Impl;

public sealed class ExpanderContext
{
    public ExpanderContext(ExpandedStringBuilder stringBuilder, ITokenValueContainer container, IInterpolatedStringSettings settings, Dictionary<string, IBlockCommand> definedCommands)
    {
        StringBuilder = stringBuilder;
        Container = container;
        Settings = settings;
        DefinedCommands = definedCommands;
    }

    public ExpandedStringBuilder StringBuilder { get; init; }
    public ITokenValueContainer Container { get; init; }
    public IInterpolatedStringSettings Settings { get; init; }
    public IDictionary<string, IBlockCommand> DefinedCommands { get; init; }
    public ExpanderValueStore ValueStore { get; } = new();

    public InterpolatedStringSegment? CurrentSegment { get; internal set; }
    public bool SkipActiveBlocks { get; set; } = false;
    internal Stack<IBlockCommand> ActiveBlocks { get; } = new Stack<IBlockCommand>();
    public IEnumerable<IBlockCommand> CurrentActiveBlocks => ActiveBlocks;
}
