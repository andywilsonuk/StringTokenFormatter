namespace StringTokenFormatter {
    public interface ITokenParser {
        SegmentedString Parse(string input);
        string RemoveTokenMarkers(string token);
    }

}
