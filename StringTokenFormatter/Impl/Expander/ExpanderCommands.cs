namespace StringTokenFormatter.Impl;

public class ExpanderCommands(IReadOnlyCollection<IExpanderCommand> commands)
{
    private readonly IReadOnlyCollection<IExpanderCommand> commands = commands;

    public void Init(ExpanderContext context) => commands.ForEach(c => c.Init(context));

    public void ExecuteUntil(Action<IExpanderCommand> action, Func<bool> predicate) => commands.ForEach(action, predicate);

    public void Finished(ExpanderContext context) => commands.ForEach(c => c.Finished(context));

    public bool HasCommand<T>() where T : IExpanderCommand => commands.OfType<T>().Any();

    public TryGetResult TryMapPseudo(ExpanderContext context, string tokenName) =>
        OrdinalValueHelper.StartsWith(tokenName, Constants.PseudoPrefix)
        ? commands.OfType<IExpanderPseudoCommands>().Select(x => x.TryMapPseudo(context, tokenName)).FirstOrDefault(x => x.IsSuccess)
        : default;
}