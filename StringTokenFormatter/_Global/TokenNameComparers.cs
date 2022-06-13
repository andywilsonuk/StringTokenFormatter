using StringTokenFormatter.Impl;
using System;
using System.Collections.Generic;

namespace StringTokenFormatter {
    public static class TokenNameComparers {

        public static ITokenNameComparer InvariantCulture { get; }
        public static ITokenNameComparer InvariantCultureIgnoreCase { get; }
        public static ITokenNameComparer Ordinal { get; }
        public static ITokenNameComparer OrdinalIgnoreCase { get; }
        public static ITokenNameComparer CurrentCulture { get; }
        public static ITokenNameComparer CurrentCultureIgnoreCase { get; }

        public static ITokenNameComparer Default { get; }

        public static ITokenNameComparer From(IEqualityComparer<string> Comparer) {
            var ret = default(ITokenNameComparer);

            if(Comparer == StringComparer.InvariantCulture) {
                ret = InvariantCulture;
            } else if (Comparer == StringComparer.InvariantCultureIgnoreCase) {
                ret = InvariantCultureIgnoreCase;
            } else if (Comparer == StringComparer.Ordinal) {
                ret = Ordinal;
            } else if (Comparer == StringComparer.OrdinalIgnoreCase) {
                ret = OrdinalIgnoreCase;
            } else if (Comparer == StringComparer.CurrentCulture) {
                ret = CurrentCulture;
            } else if (Comparer == StringComparer.CurrentCultureIgnoreCase) {
                ret = CurrentCultureIgnoreCase;
            }

            if (ret is null) {
                ret = new StringComparerTokenNameComparerImpl(Comparer);
            }

            return ret;
        }

        static TokenNameComparers() {
            InvariantCulture = From(StringComparer.InvariantCulture);
            InvariantCultureIgnoreCase = From(StringComparer.InvariantCultureIgnoreCase);
            Ordinal = From(StringComparer.Ordinal);
            OrdinalIgnoreCase = From(StringComparer.OrdinalIgnoreCase);
            CurrentCulture = From(StringComparer.CurrentCulture);
            CurrentCultureIgnoreCase = From(StringComparer.CurrentCultureIgnoreCase);

            Default = CurrentCultureIgnoreCase;
        }

    }



}
