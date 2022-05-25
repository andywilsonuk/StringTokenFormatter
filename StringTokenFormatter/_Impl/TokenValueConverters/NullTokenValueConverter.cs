using StringTokenFormatter.Impl;

namespace StringTokenFormatter.Impl.TokenValueConverters {

    /// <summary>
    /// A short-circuit value converter that triggers when the value is null.
    /// </summary>
    public sealed class NullTokenValueConverter : ITokenValueConverter {
        public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
            mapped = null;
            return value == null;
        }

        private NullTokenValueConverter() { }

        public static NullTokenValueConverter Instance { get; } = new NullTokenValueConverter();

    }

}
