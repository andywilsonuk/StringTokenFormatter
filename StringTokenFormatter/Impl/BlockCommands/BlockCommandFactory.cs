namespace StringTokenFormatter.Impl;

public static class BlockCommandFactory
{
    public static ConditionalBlockCommand Conditional { get; } = new ConditionalBlockCommand();
    public static LoopBlockCommand Loop { get; } = new LoopBlockCommand();
}