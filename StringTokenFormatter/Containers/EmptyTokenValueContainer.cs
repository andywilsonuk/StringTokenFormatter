namespace StringTokenFormatter {
    public class EmptyTokenValueContainer : ITokenValueContainer {

        public static EmptyTokenValueContainer Instance { get; private set; } = new EmptyTokenValueContainer();

        private EmptyTokenValueContainer() {

        }

        public bool TryMap(IMatchedToken matchedToken, out object? mapped) {
            mapped = null;
            return false;
        }
    }
}
