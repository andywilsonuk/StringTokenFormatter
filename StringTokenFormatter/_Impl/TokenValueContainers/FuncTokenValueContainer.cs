using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.TokenNameComparers;
using System;

namespace StringTokenFormatter.Impl.TokenValueContainers {

    /// <summary>
    /// This <see cref="ITokenValueContainer"/> resolve values using the specified delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncTokenValueContainer<T> : ITokenValueContainer {

        protected readonly ITokenNameComparer nameComparer;
        protected readonly Func<string, ITokenNameComparer, TryGetResult> resolver;

        
        public FuncTokenValueContainer(Func<string, T> valueResolver, ITokenNameComparer nameComparer) {
            
            
            this.resolver = (TokenName, TokenNameComparer) => {
                var ret = default(TryGetResult);
                
                if(valueResolver(TokenName) is { } V1) {
                    ret = TryGetResult.Success(V1);
                }

                return ret;
            };
            this.nameComparer = nameComparer;
        }
        public FuncTokenValueContainer(Func<string, ITokenNameComparer, T> valueResolver, ITokenNameComparer nameComparer) {
            this.resolver = (TokenName, TokenNameComparer) => {
                var ret = default(TryGetResult);

                if (valueResolver(TokenName, TokenNameComparer) is { } V1) {
                    ret = TryGetResult.Success(V1);
                }

                return ret;
            };
            this.nameComparer = nameComparer;
        }


        public FuncTokenValueContainer(Func<string, ITokenNameComparer, TryGetResult> valueResolver, ITokenNameComparer nameComparer) {
            this.resolver = valueResolver;
            this.nameComparer = nameComparer;
        }

        public TryGetResult TryMap(ITokenMatch matchedToken) {
            var ret = resolver(matchedToken.Token, nameComparer);
            return ret;
        }
    }
}

    

