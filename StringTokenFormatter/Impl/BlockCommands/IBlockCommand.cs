namespace StringTokenFormatter.Impl;

public interface IBlockCommand
{
    public void Evaluate(ExpanderContext context);
    public void Finished(ExpanderContext context);
}