namespace StringTokenFormatter.Impl;

public interface IBlockCommand
{
    public void Init(ExpanderContext context);
    public void Evaluate(ExpanderContext context);
    public void Finished(ExpanderContext context);
}