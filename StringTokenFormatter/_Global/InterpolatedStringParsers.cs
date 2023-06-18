using StringTokenFormatter.Impl;

namespace StringTokenFormatter; 
public static class InterpolatedStringParsers {
    public static IInterpolatedStringParser Curly { get; }
    public static IInterpolatedStringParser DollarCurly { get; }

    public static IInterpolatedStringParser Round { get; }
    public static IInterpolatedStringParser DollarRound { get; }
    public static IInterpolatedStringParser DollarRoundAlternative { get; }

    public static IInterpolatedStringParser Default { get; }


    public static IInterpolatedStringParser Create(ITokenSyntax Options) {
        var ret = default(IInterpolatedStringParser);

        if (Options == TokenSyntaxes.Curly) {
            ret = Curly;
        } else if (Options == TokenSyntaxes.DollarCurly) {
            ret = DollarCurly;
        } else if (Options == TokenSyntaxes.Round) {
            ret = Round;
        } else if (Options == TokenSyntaxes.DollarRound) {
            ret = DollarRound;
        } else if (Options == TokenSyntaxes.DollarRoundAlternative) {
            ret = DollarRoundAlternative;
        }
        
        ret ??= new InterpolatedStringParserImpl(Options);

        return ret;
    }

    static InterpolatedStringParsers() {
        Curly = new InterpolatedStringParserImpl(TokenSyntaxes.Curly);
        DollarCurly = new InterpolatedStringParserImpl(TokenSyntaxes.DollarCurly);
        Round = new InterpolatedStringParserImpl(TokenSyntaxes.Round);
        DollarRound = new InterpolatedStringParserImpl(TokenSyntaxes.DollarRound);
        DollarRoundAlternative = new InterpolatedStringParserImpl(TokenSyntaxes.DollarRoundAlternative);

        Default = Curly;
    }

}
