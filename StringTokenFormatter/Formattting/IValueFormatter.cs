namespace StringTokenFormatter
{
    public interface IValueFormatter
    {
        string Format(TokenMatchingSegment token, object value);
    }
}
