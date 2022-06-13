namespace StringTokenFormatter {

    public interface ITokenValueContainer {
        TryGetResult TryMap(ITokenMatch matchedToken);
    }
}