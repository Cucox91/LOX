namespace CSLOX
{
    public class Token
    {
        public TokenType TokenType { get; set; }
        public string Lexeme { get; set; }
        public object? Literal { get; set; }
        public int Line { get; set; }

        public Token(TokenType tokenType, string lexeme, object? literal, int line)
        {
            TokenType = tokenType;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{TokenType} {Lexeme} {Literal}";
        }
    }
}

/*
    Notes:
    Some implementations stores location at two values. One being the offset from the beginning of the source file
    to the beginning of the lexeme and the other the lenght of the lexeme.
    This is not overhead because either way the scanner needs to know this.
    This values then can ve used to display line and column position.
*/

