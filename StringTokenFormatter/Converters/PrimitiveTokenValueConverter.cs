namespace StringTokenFormatter {

    /// <summary>
    /// A short-circuit value converter that triggers when the value is a primitive.
    /// </summary>
    public sealed class PrimitiveTokenValueConverter : ITokenValueConverter {
        public bool TryConvert(IMatchedToken token, object? value, out object? mapped) {
            var ret = false;
            mapped = null;

            if (value != null && (value is string || value.GetType().IsValueType)) {
                mapped = value;
                ret = true;
            }

            return ret;
        }

        private PrimitiveTokenValueConverter() { }
        public static PrimitiveTokenValueConverter Instance { get; private set; } = new PrimitiveTokenValueConverter();

    }
}
