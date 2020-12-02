using System;
using System.Collections.Generic;
using System.Text;

namespace StringTokenFormatter {
    public static class TokenNameComparer {


        private static ITokenNameComparer __Default;
        public static ITokenNameComparer Default {
            get => __Default;
            set => __Default = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static StringComparerTokenNameComparer CurrentCulture { get; private set; }
        public static StringComparerTokenNameComparer CurrentCultureIgnoreCase { get; private set; }
        public static StringComparerTokenNameComparer InvariantCulture { get; private set; }
        public static StringComparerTokenNameComparer InvariantCultureIgnoreCase { get; private set; }
        public static StringComparerTokenNameComparer Ordinal { get; private set; }
        public static StringComparerTokenNameComparer OrdinalIgnoreCase { get; private set; }

        static TokenNameComparer() {
            CurrentCulture = new StringComparerTokenNameComparer(StringComparer.CurrentCulture);
            CurrentCultureIgnoreCase = new StringComparerTokenNameComparer(StringComparer.CurrentCultureIgnoreCase);
            InvariantCulture = new StringComparerTokenNameComparer(StringComparer.InvariantCulture);
            InvariantCultureIgnoreCase = new StringComparerTokenNameComparer(StringComparer.InvariantCultureIgnoreCase);
            Ordinal = new StringComparerTokenNameComparer(StringComparer.Ordinal);
            OrdinalIgnoreCase = new StringComparerTokenNameComparer(StringComparer.OrdinalIgnoreCase);

            __Default = InvariantCultureIgnoreCase;
        }

    }

    public interface ITokenNameComparer {
        IEqualityComparer<string> Comparer { get; }
    }

    public class StringComparerTokenNameComparer : ITokenNameComparer {

        public StringComparerTokenNameComparer(IEqualityComparer<string> Comparer) {
            this.Comparer = Comparer;
        }

        public IEqualityComparer<string> Comparer { get; private set; }
    }



}
