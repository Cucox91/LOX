namespace CSLOX
{
    public class Token
    {
        private readonly TokenType _tokenType;
        private readonly string _lexeme;
        private readonly object? _literal;
        private readonly int _line;

        public Token(TokenType tokenType, string lexeme, object? literal, int line)
        {
            _tokenType = tokenType;
            _lexeme = lexeme;
            _literal = literal;
            _line = line;
        }

        public override string ToString()
        {
            return $"{_tokenType} {_lexeme} {_literal}";
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

