namespace StringTokenFormatter.Impl;

public interface IBlockCommand
{
    public string StartCommandName { get; }
    public string EndCommandName { get; }

    public void Start(ExpanderContext context, InterpolatedStringBlockSegment blockSegment);
    public void End(ExpanderContext context, InterpolatedStringBlockSegment blockSegment);
    public void Evaluate(ExpanderContext context);
}