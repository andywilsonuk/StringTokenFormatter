namespace StringTokenFormatter {

    /// <summary>
    /// A short-circuit value converter that triggers when the value is null.
    /// </summary>
    public sealed class NullTokenValueConverter : ITokenValueConverter {
        public bool TryConvert(IMatchedToken token, object value, out object mapped) {
            mapped = null;
            return value == null;
        }

        private NullTokenValueConverter() { }

        public static NullTokenValueConverter Instance { get; private set; } = new NullTokenValueConverter();

    }

}
