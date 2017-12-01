namespace StringTokenFormatter
{
    public class TokenToNullValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            mapped = null;
            return value == null;
        }
    }
}
