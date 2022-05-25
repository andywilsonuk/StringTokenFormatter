using StringTokenFormatter.Impl;
using System;

namespace StringTokenFormatter.Impl.TokenValueConverters {

    /// <summary>
    /// Converts objects that are a Lazy<typeparamref name="T"/> to simply their <typeparamref name="T"/> value.
    /// </summary>
    /// <typeparam name="T">The type of the Lazy.</typeparam>
    public sealed class LazyTokenValueConverter<T> : ITokenValueConverter {
        public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
            if (value is Lazy<T> lazy) {
                mapped = lazy.Value;
                return true;
            }
            mapped = null;
            return false;
        }

        private LazyTokenValueConverter() { }

        public static LazyTokenValueConverter<T> Instance { get; } = new LazyTokenValueConverter<T>();

    }
}
