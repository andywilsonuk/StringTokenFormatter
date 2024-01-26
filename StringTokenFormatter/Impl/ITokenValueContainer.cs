namespace StringTokenFormatter.Impl;

public interface ITokenValueContainer
{
    TryGetResult TryMap(string token);
}
public interface ISequenceTokenValueContainer
{
    int Count { get; }
    TryGetResult TryMapForIndex(string token, int index);
}
