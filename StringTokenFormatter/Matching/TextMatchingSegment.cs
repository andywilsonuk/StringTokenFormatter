using System;

namespace StringTokenFormatter
{
    public class TextMatchingSegment : IMatchingSegment
    {
        public TextMatchingSegment(string text)
        {
            Original = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Original { get; }
        public override string ToString() => Original;
    }
}