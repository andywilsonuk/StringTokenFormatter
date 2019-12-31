using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter {

    /// <summary>
    /// This Value Container searches all child containers for the provided token value and returns the first value found. 
    /// </summary>
    public class CompositeTokenValueContainer : ITokenValueContainer {
        protected readonly List<ITokenValueContainer> containers;

        public CompositeTokenValueContainer(IEnumerable<ITokenValueContainer> containers) {
            containers = containers ?? throw new ArgumentNullException(nameof(containers));

            this.containers = containers.Where(x => x != default).ToList();

        }

        public virtual bool TryMap(IMatchedToken matchedToken, out object mapped) {
            foreach (var container in containers) {
                if (container.TryMap(matchedToken, out mapped)) return true;
            }
            mapped = null;
            return false;
        }
    }

}
