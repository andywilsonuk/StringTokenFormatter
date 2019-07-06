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

        
        public FuncTokenValueContainer(Func<string, T> ValueResolver, ITokenParser Parser = default) {
            ValueResolver = ValueResolver ?? throw new ArgumentNullException(nameof(ValueResolver));

            this.resolver = (x, y) => ValueResolver(x);
            this.parser = Parser ?? TokenParser.Default;
        }


        public FuncTokenValueContainer(Func<string, ITokenParser, T> ValueResolver, ITokenParser Parser = default) {
            this.resolver = ValueResolver ?? throw new ArgumentNullException(nameof(ValueResolver));
            this.parser = Parser ?? TokenParser.Default;
        }

        public bool TryMap(IMatchedToken matchedToken, out object mapped) {
            mapped = resolver(matchedToken.Token, parser);

            return mapped != null;
        }
    }
}

    

