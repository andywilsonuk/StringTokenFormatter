using System.Threading.Tasks;

namespace StringTokenFormatter.Impl {
    public interface ITokenValueContainerAsync {
        Task<TryGetResult> TryMapAsync(ITokenMatch matchedToken);
    }
}