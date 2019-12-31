using System;

namespace StringTokenFormatter {
    /// <summary>
    /// Prevents replacement of null token values.
    /// </summary>
    public class IgnoreNullTokenValueContainer : ITokenValueContainer {
        protected readonly ITokenValueContainer child;
        public IgnoreNullTokenValueContainer(ITokenValueContainer child) {
            child = child ?? throw new ArgumentNullException(nameof(child));
            
            this.child = child;
        }
        
        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            var ret = child.TryMap(matchedToken, out mapped);

            if (ret) {
                if(mapped == default) {
                    ret = false;
                    mapped = null;
                }
            }
            return ret;
        }
    }

}
