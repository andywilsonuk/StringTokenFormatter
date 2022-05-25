namespace StringTokenFormatter.Impl {

    public interface ITokenValueContainer {
        TryGetResult TryMap(ITokenMatch matchedToken);
    }
}