using StringTokenFormatter.Impl;
using System;

namespace StringTokenFormatter.Impl.TokenValueConverters {
    /// <summary>
    /// A value converter that will execute a Func<typeparamref name="T"/> and return its value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FuncTokenValueConverter<T> : ITokenValueConverter {
        public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
            if (value is Func<T> func) {
                mapped = func();
                return true;
            }
            mapped = null;
            return false;
        }

        private FuncTokenValueConverter() { }

        public static FuncTokenValueConverter<T> Instance { get; } = new FuncTokenValueConverter<T>();

    }
}
