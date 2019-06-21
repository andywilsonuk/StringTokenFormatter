namespace StringTokenFormatter
{
    public class TokenToPrimitiveValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var ret = false;
            mapped = null;

            if(value != null && (value is string || value.GetType().IsValueType)) {
                mapped = value;
                ret = true;
            }

            return ret;
        }
    }
}
