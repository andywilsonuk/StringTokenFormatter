using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.TokenNameComparers;
using System;

namespace StringTokenFormatter {
    public static class TokenNameComparers {

        public static ITokenNameComparer InvariantCulture { get; }
        public static ITokenNameComparer InvariantCultureIgnoreCase { get; }
        public static ITokenNameComparer Ordinal { get; }
        public static ITokenNameComparer OrdinalIgnoreCase { get; }
        public static ITokenNameComparer CurrentCulture { get; }
        public static ITokenNameComparer CurrentCultureIgnoreCase { get; }

        public static ITokenNameComparer Default { get; }

        static TokenNameComparers() {
            InvariantCulture = new StringComparerTokenNameComparer(StringComparer.InvariantCulture);
            InvariantCultureIgnoreCase = new StringComparerTokenNameComparer(StringComparer.InvariantCultureIgnoreCase);
            Ordinal = new StringComparerTokenNameComparer(StringComparer.Ordinal);
            OrdinalIgnoreCase = new StringComparerTokenNameComparer(StringComparer.OrdinalIgnoreCase);
            CurrentCulture = new StringComparerTokenNameComparer(StringComparer.CurrentCulture);
            CurrentCultureIgnoreCase = new StringComparerTokenNameComparer(StringComparer.CurrentCultureIgnoreCase);

            Default = CurrentCultureIgnoreCase;
        }

    }



}
