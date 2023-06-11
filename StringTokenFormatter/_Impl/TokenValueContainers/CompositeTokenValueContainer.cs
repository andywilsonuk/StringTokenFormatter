namespace StringTokenFormatter.Impl.TokenValueContainers
{

    /// <summary>
    /// This Value Container searches all child containers for the provided token value and returns the first value found. 
    /// </summary>
    internal class CompositeTokenValueContainerImpl : ITokenValueContainer {
        protected ITokenValueContainer[] containers;

        public CompositeTokenValueContainerImpl(IEnumerable<ITokenValueContainer> containers) {
            this.containers = containers.Where(x => x is { }).ToArray();
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = default(TryGetResult);
            foreach (var container in containers) {
                ret = container.TryMap(matchedToken);
                if (ret.IsSuccess) {
                    break;
                }
            }

            return ret;
        }
    }

}
