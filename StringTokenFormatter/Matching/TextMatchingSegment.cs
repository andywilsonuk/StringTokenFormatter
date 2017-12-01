namespace StringTokenFormatter
{
    public class TextMatchingSegment : IMatchingSegment
    {
        public TextMatchingSegment(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }

        public override string ToString() => Text;
    }
}