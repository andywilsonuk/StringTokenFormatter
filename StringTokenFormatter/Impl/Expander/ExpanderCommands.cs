namespace StringTokenFormatter.Impl;

public class ExpanderCommands
{
    private readonly IReadOnlyCollection<IExpanderCommand> commands;

    public ExpanderCommands(IReadOnlyCollection<IExpanderCommand> commands)
    {
        this.commands = commands;
    }

    public void Init(ExpanderContext context) => commands.ForEach(c => c.Init(context));

    public void ExecuteUntil(Action<IExpanderCommand> action, Func<bool> predicate) => commands.ForEach(action, predicate);

    public void Finished(ExpanderContext context) => commands.ForEach(c => c.Finished(context));
}