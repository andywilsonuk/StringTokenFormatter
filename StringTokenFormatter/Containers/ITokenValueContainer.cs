namespace StringTokenFormatter {

    public interface ITokenValueContainer {
        bool TryMap(IMatchedToken matchedToken, out object? mapped);
    }

}