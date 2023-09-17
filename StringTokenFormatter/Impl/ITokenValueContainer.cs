namespace StringTokenFormatter.Impl;

public interface ITokenValueContainer
{
    TryGetResult TryMap(string token);
}
