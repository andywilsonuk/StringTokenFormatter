using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTokenFormatter {

    /// <summary>
    /// This <see cref="ITokenValueContainer"/> resolve values using the specified delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncTokenValueContainer<T> : ITokenValueContainer {

        private readonly ITokenParser parser;
        private readonly Func<string, ITokenParser, T> resolver;

        
        public FuncTokenValueContainer(Func<string, T> valueResolver, ITokenParser parser = default) {
            valueResolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));

            this.resolver = (x, y) => valueResolver(x);
            this.parser = parser ?? TokenParser.Default;
        }


        public FuncTokenValueContainer(Func<string, ITokenParser, T> valueResolver, ITokenParser parser = default) {
            this.resolver = valueResolver ?? throw new ArgumentNullException(nameof(valueResolver));
            this.parser = parser ?? TokenParser.Default;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            mapped = resolver(matchedToken.Token, parser);

            return mapped != null;
        }
    }
}

    

