namespace StringTokenFormatter.Impl;

public class ExpanderContext
{
    public ExpanderContext(ExpandedStringBuilder stringBuilder, ITokenValueContainer container, IInterpolatedStringSettings settings, Dictionary<string, ICommandBlock> definedCommands)
    {
        StringBuilder = stringBuilder;
        Container = container;
        Settings = settings;
        DefinedCommands = definedCommands;
    }

    public ExpandedStringBuilder StringBuilder { get; init; }
    public ITokenValueContainer Container { get; init; }
    public IInterpolatedStringSettings Settings { get; init; }
    public IDictionary<string, ICommandBlock> DefinedCommands { get; init; }
    public ExpanderValueStore ValueStore { get; } = new();

    public InterpolatedStringSegment? CurrentSegment { get; internal set; }
    public bool SkipActiveBlocks { get; set; } = false;
    internal Stack<ICommandBlock> ActiveBlocks { get; } = new Stack<ICommandBlock>();
}
