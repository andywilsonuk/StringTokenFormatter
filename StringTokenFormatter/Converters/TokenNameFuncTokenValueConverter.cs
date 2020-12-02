using System;

namespace StringTokenFormatter {


    /// <summary>
    /// A Value Converter that will execute a Func<string, <typeparamref name="T"/>> and return its value.  The first prarameter will be the token name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class TokenNameFuncTokenValueConverter<T> : ITokenValueConverter {
        public bool TryConvert(IMatchedToken token, object? value, out object? mapped) {

            if (value is Func<string, T> func) {
                mapped = func(token.Token);
                return true;
            }
            mapped = null;
            return false;
        }

        private TokenNameFuncTokenValueConverter() { }

        public static TokenNameFuncTokenValueConverter<T> Instance { get; private set; } = new TokenNameFuncTokenValueConverter<T>();

    }
}
