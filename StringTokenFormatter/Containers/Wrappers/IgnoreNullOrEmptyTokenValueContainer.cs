using System;

namespace StringTokenFormatter {
    /// <summary>
    /// Prevents replacement of null or empty token values.
    /// </summary>
    public class IgnoreNullOrEmptyTokenValueContainer : ITokenValueContainer {
        protected readonly ITokenValueContainer child;
        public IgnoreNullOrEmptyTokenValueContainer(ITokenValueContainer child) {
            child = child ?? throw new ArgumentNullException(nameof(child));

            this.child = child;
        }

        public bool TryMap(IMatchedToken matchedToken, out object? mapped) {
            var ret = child.TryMap(matchedToken, out mapped);

            if (ret) {
                if (mapped == default || mapped is string V1 && V1.Length == 0) {
                    ret = false;
                    mapped = null;
                }
            }
            return ret;
        }
    }

}
