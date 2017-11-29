using System;

namespace StringTokenFormatter
{
    public class TokenMatchingSegment : IMatchingSegment, IMatchedToken
    {
        public TokenMatchingSegment(string original, string token, string padding, string format)
        {
            Original = original ?? throw new ArgumentNullException(nameof(original));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Padding = padding;
            Format = format;
        }

        public string Original { get; }
        public string Token { get; }
        public string Padding { get; }
        public string Format { get; }
    }
}