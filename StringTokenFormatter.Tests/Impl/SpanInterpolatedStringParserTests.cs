namespace StringTokenFormatter.Tests;

public class SpanInterpolatedStringParserTests
{
    [Fact]
    public void Parse_MatchesRegexParserAcrossSupportedComponentsAndSyntax()
    {
        string[] sources =
        [
            string.Empty,
            "plain text without tokens",
            "before {name} after",
            "{name,10:D}",
            "{:command}",
            "{:command,token}",
            "{:command:line1\nline2,more}",
            "{::pseudo,10:D}",
            "escaped {{name} and actual {value}",
            "{first} middle {second,8:X}",
            "unclosed {token",
            "(name,10:D)",
            "((name) and (value)",
            "<%name%>",
            "<<%name%> and <%value%>"
        ];
        StringTokenFormatterSettings[] settings =
        [
            new(),
            new() { Syntax = CommonTokenSyntax.Round },
            new() { Syntax = new TokenSyntax("<%", "%>", "<<%") }
        ];

        foreach (StringTokenFormatterSettings currentSettings in settings)
        {
            foreach (string source in sources)
            {
                InterpolatedString expected = RegexInterpolatedStringParser.Parse(source, currentSettings);
                InterpolatedString actual = SpanInterpolatedStringParser.Parse(source, currentSettings);

                Assert.Equal(expected.Segments, actual.Segments);
                Assert.Equal(expected.ToRawString(), actual.ToRawString());
            }
        }
    }

    [Fact]
    public void Parse_BlankToken_ThrowsParserException()
    {
        StringTokenFormatterSettings settings = new();

        Assert.Throws<ParserException>(() => SpanInterpolatedStringParser.Parse("{}", settings));
    }
}
