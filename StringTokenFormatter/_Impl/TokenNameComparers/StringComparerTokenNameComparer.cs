using StringTokenFormatter.Impl;
using System.Collections.Generic;

namespace StringTokenFormatter.Impl.TokenNameComparers {
    internal class StringComparerTokenNameComparer : ITokenNameComparer {

        public StringComparerTokenNameComparer(IEqualityComparer<string> Comparer) {
            this.Comparer = Comparer;
        }

        public IEqualityComparer<string> Comparer { get; }

        bool IEqualityComparer<string>.Equals(string x, string y) {
            return Comparer.Equals(x, y);
        }

        int IEqualityComparer<string>.GetHashCode(string obj) {
            return Comparer.GetHashCode(obj);
        }
    }



}
