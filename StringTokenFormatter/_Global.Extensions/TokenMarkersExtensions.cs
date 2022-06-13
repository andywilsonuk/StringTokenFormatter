namespace StringTokenFormatter {
    public static class TokenMarkersExtensions {
        /// <summary>
        /// Converts "Token" to "{Token}"
        /// </summary>
        public static string Wrap(this ITokenSyntax This, string TokenName) {
           
            var Start = This.StartToken;
            var End = This.EndToken;
            var ret = $@"{Start}{TokenName}{End}";

            return ret;
        }

        /// <summary>
        /// Converts "{Token}" to "Token"
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static string Unwrap(this ITokenSyntax This, string Token) {
            var ret = Token;
            if (ret.StartsWith(This.StartToken) && !ret.StartsWith(This.StartTokenEscaped)) {
                ret = ret.Remove(0, This.StartToken.Length);

                if (Token.EndsWith(This.EndToken)) {
                    ret = ret.Remove(ret.Length - This.EndToken.Length);
                }

            }
            return ret;
        }

    }
}
