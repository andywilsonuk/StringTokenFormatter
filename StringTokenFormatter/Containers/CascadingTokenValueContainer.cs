using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class CascadingTokenValueContainer : ITokenValueContainer
    {
        private readonly ICollection<ITokenValueContainer> containers;

        public CascadingTokenValueContainer(ICollection<ITokenValueContainer> containers)
        {
            this.containers = containers ?? throw new ArgumentNullException(nameof(containers));
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped)
        {
            foreach (var container in containers)
            {
                if (container.TryMap(matchedToken, out mapped)) return true;
            }
            mapped = null;
            return false;
        }
    }
}
