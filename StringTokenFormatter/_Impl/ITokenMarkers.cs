namespace StringTokenFormatter {

    public interface ITokenSyntax {
        string StartToken { get; }
        string EndToken { get; }
        string StartTokenEscaped { get; }
    }

}
