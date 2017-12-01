namespace StringTokenFormatter
{
    public interface ITokenToValueMapper
    {
        bool TryMap(IMatchedToken matchedToken, object value, out object mapped);
    }
}
