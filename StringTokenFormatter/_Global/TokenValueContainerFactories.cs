using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.TokenValueContainerFactories;
using StringTokenFormatter.Impl.TokenValueContainers;

namespace StringTokenFormatter {
    public static class TokenValueContainerFactories {

        public static ITokenValueContainerFactory InvariantCulture { get; }
        public static ITokenValueContainerFactory InvariantCultureIgnoreCase { get; }
        public static ITokenValueContainerFactory Ordinal { get; }
        public static ITokenValueContainerFactory OrdinalIgnoreCase { get; }
        public static ITokenValueContainerFactory CurrentCulture { get; }
        public static ITokenValueContainerFactory CurrentCultureIgnoreCase { get; }

        public static ITokenValueContainerFactory Default { get; }

        public static ITokenValueContainerFactory Create(ITokenNameComparer NameComparer) {
            var ret = default(ITokenValueContainerFactory);

            if(NameComparer == TokenNameComparers.InvariantCulture) {
                ret = InvariantCulture;
            } else if (NameComparer == TokenNameComparers.InvariantCultureIgnoreCase) {
                ret = InvariantCultureIgnoreCase;
            } else if (NameComparer == TokenNameComparers.Ordinal) {
                ret = Ordinal;
            } else if (NameComparer == TokenNameComparers.OrdinalIgnoreCase) {
                ret = OrdinalIgnoreCase;
            } else if (NameComparer == TokenNameComparers.CurrentCulture) {
                ret = CurrentCulture;
            } else if (NameComparer == TokenNameComparers.CurrentCultureIgnoreCase) {
                ret = CurrentCultureIgnoreCase;
            }
            
            if(ret == null) { 
                ret = new TokenValueContainerFactory(NameComparer);
            }

            return ret;
        }

        static TokenValueContainerFactories() {
            InvariantCulture = Create(TokenNameComparers.InvariantCulture);
            InvariantCultureIgnoreCase = Create(TokenNameComparers.InvariantCultureIgnoreCase);
            CurrentCulture = Create(TokenNameComparers.CurrentCulture);
            CurrentCultureIgnoreCase = Create(TokenNameComparers.CurrentCultureIgnoreCase);
            Ordinal = Create(TokenNameComparers.Ordinal);
            OrdinalIgnoreCase = Create(TokenNameComparers.OrdinalIgnoreCase);

            Default = CurrentCultureIgnoreCase;
        }
    }
}