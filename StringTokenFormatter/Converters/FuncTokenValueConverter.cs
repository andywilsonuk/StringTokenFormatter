using System;

namespace StringTokenFormatter {
    /// <summary>
    /// A value converter that will execute a Func<typeparamref name="T"/> and return its value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FuncTokenValueConverter<T> : ITokenValueConverter {
        public bool TryConvert(IMatchedToken token, object value, out object mapped) {
            if (value is Func<T> func) {
                mapped = func();
                return true;
            }
            mapped = null;
            return false;
        }

        private FuncTokenValueConverter() { }

        public static FuncTokenValueConverter<T> Instance { get; private set; } = new FuncTokenValueConverter<T>();

    }
}
