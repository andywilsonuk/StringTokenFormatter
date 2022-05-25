using StringTokenFormatter.Impl;
using System;

namespace StringTokenFormatter.Impl.TokenValueContainers {
    /// <summary>
    /// Prevents replacement of null or empty token values.
    /// </summary>
    public class IgnoreNullOrEmptyTokenValueContainer : ITokenValueContainer {
        protected readonly ITokenValueContainer child;
        public IgnoreNullOrEmptyTokenValueContainer(ITokenValueContainer child) {
            this.child = child;
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = child.TryMap(matchedToken);

            if (ret.IsSuccess && ret.Value is null or string { Length: 0}) {
                ret = default;
            }
            return ret;
        }
    }

}
