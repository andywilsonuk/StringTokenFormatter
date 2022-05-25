using StringTokenFormatter.Impl;
using StringTokenFormatter.Impl.InterpolatedStringParsers;

namespace StringTokenFormatter {
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
            
            if(ret is null) {
                ret = new InterpolatedStringParser(Options);
            }

            return ret;
        }

        static InterpolatedStringParsers() {
            Curly = new InterpolatedStringParser(TokenSyntaxes.Curly);
            DollarCurly = new InterpolatedStringParser(TokenSyntaxes.DollarCurly);
            Round = new InterpolatedStringParser(TokenSyntaxes.Round);
            DollarRound = new InterpolatedStringParser(TokenSyntaxes.DollarRound);
            DollarRoundAlternative = new InterpolatedStringParser(TokenSyntaxes.DollarRoundAlternative);

            Default = Curly;
        }

    }

}
