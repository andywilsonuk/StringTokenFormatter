using System;

namespace StringTokenFormatter {

    /// <summary>
    /// This <see cref="ITokenValueContainer"/> resolve values using the specified delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncTokenValueContainer<T> : ITokenValueContainer {

        protected readonly ITokenNameComparer nameComparer;
        protected readonly Func<string, ITokenNameComparer, T> resolver;

        
        public FuncTokenValueContainer(Func<string, T> valueResolver, ITokenNameComparer? nameComparer = default) {
            valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));

            this.resolver = (x, y) => valueResolver(x);
            this.nameComparer = nameComparer ?? TokenNameComparer.Default;
        }


        public FuncTokenValueContainer(Func<string, ITokenNameComparer, T> valueResolver, ITokenNameComparer? nameComparer = default) {
            this.resolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            this.nameComparer = nameComparer ?? TokenNameComparer.Default;
        }

        public virtual bool TryMap(IMatchedToken matchedToken, out object? mapped) {
            mapped = resolver(matchedToken.Token, nameComparer);

            return mapped != null;
        }
    }
}

    

