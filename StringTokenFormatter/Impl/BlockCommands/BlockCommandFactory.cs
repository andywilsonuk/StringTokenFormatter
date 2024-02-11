namespace StringTokenFormatter.Impl;

public static class BlockCommandFactory
{
    public static ConditionalBlockCommand Conditional { get; } = new();
    public static LoopBlockCommand Loop { get; } = new();
    public static MapCommand Map { get; } = new();
}