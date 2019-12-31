namespace StringTokenFormatter {

    public interface ISegment {
        string Original { get; }

        string Evaluate(ITokenValueContainer container, ITokenValueFormatter formatter, ITokenValueConverter converter);

    }

}
