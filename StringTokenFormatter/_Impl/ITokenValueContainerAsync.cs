namespace StringTokenFormatter;

public interface ITokenValueContainerAsync {
    Task<TryGetResult> TryMapAsync(ITokenMatch matchedToken);
}