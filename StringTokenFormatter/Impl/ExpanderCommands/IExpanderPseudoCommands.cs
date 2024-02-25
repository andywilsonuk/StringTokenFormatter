namespace StringTokenFormatter.Impl;

public interface IExpanderPseudoCommands
{
    TryGetResult TryMapPseudo(ExpanderContext context, string tokenName);
}