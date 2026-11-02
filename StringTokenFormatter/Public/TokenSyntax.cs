namespace StringTokenFormatter;

public readonly record struct TokenSyntax(string Start, string End, string EscapedStart);

public static class TokenSyntaxExtensions
{
    extension(TokenSyntax syntax)
    {
        /// <summary>
        /// Returns the token name wrapped within the start and end syntax.
        /// </summary>
        public string Tokenize(string tokenName) => $"{syntax.Start}{tokenName}{syntax.End}";

        /// <summary>
        /// Escapes token start markers in the text so they are treated as literal text.
        /// </summary>
        public string Escape(string value) => value.Replace(syntax.Start, syntax.EscapedStart);

        /// <summary>
        /// Asserts that the syntax is properly configured
        /// </summary>
        public TokenSyntax Validate()
        {
            (string? start, string? end, string? escapedStart) = syntax;
            Guard.NotEmpty(start, nameof(start));
            Guard.NotEmpty(end, nameof(end));
            Guard.NotEmpty(escapedStart, nameof(escapedStart));
            if (new HashSet<string> { start, syntax.End, escapedStart }.Count != 3) { throw new ArgumentException($"Duplicate token marker detected for syntax {syntax}"); }
            return syntax;
        }
    }
}