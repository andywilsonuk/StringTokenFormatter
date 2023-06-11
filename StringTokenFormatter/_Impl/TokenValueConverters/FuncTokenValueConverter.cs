namespace StringTokenFormatter.Impl.TokenValueConverters
{
    /// <summary>
    /// A value converter that will execute a Func<typeparamref name="T"/> and return its value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class FuncTokenValueConverterImpl<T> : ITokenValueConverter {
        public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
            if (value is Func<T> func) {
                mapped = func();
                return true;
            }
            mapped = null;
            return false;
        }

        private FuncTokenValueConverterImpl() { }

        public static FuncTokenValueConverterImpl<T> Instance { get; } = new FuncTokenValueConverterImpl<T>();

    }
}
