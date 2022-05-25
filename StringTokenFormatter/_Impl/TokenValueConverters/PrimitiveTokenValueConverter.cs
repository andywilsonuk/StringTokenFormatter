using StringTokenFormatter.Impl;

namespace StringTokenFormatter.Impl.TokenValueConverters {

    /// <summary>
    /// A short-circuit value converter that triggers when the value is a primitive.
    /// </summary>
    public sealed class PrimitiveTokenValueConverter : ITokenValueConverter {
        public bool TryConvert(ITokenMatch token, object? value, out object? mapped) {
            var ret = false;
            mapped = null;

            if (value != null && (value is string || value.GetType().IsValueType)) {
                mapped = value;
                ret = true;
            }

            return ret;
        }

        private PrimitiveTokenValueConverter() { }
        public static PrimitiveTokenValueConverter Instance { get; } = new PrimitiveTokenValueConverter();

    }
}
