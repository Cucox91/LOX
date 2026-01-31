namespace CSLOX
{
    public class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = [];

        private int _start;
        private int _current;
        private int _line;

        private readonly Dictionary<string, TokenType?> _keywords =
            new Dictionary<string, TokenType?>();

        public Scanner(string source)
        {
            _source = source;
            _start = 0;
            _current = 0;
            _line = 1;

            // Adding Keywords to Dictionary.
            _keywords.Add("and", TokenType.AND);
            _keywords.Add("class", TokenType.CLASS);
            _keywords.Add("else", TokenType.ELSE);
            _keywords.Add("false", TokenType.FALSE);
            _keywords.Add("for", TokenType.FOR);
            _keywords.Add("fun", TokenType.FUN);
            _keywords.Add("if", TokenType.IF);
            _keywords.Add("nil", TokenType.NIL);
            _keywords.Add("or", TokenType.OR);
            _keywords.Add("print", TokenType.PRINT);
            _keywords.Add("return", TokenType.RETURN);
            _keywords.Add("super", TokenType.SUPER);
            _keywords.Add("this", TokenType.THIS);
            _keywords.Add("true", TokenType.TRUE);
            _keywords.Add("var", TokenType.VAR);
            _keywords.Add("while", TokenType.WHILE);
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                // Single Characters:
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                // Operators:
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                // Comments and Divide (/)
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                // Useless Characters:
                case ' ':
                case '\r':
                case '\t':
                    // Ignore Whitespaces.
                    break;
                case '\n':
                    _line++;
                    break;
                // Strings Literals.
                case '"':
                    AddString();
                    break;
                // The code below for the 'or' is not valid.
                // There is a principle called 'Maximal Munch'.
                //case 'o':
                //if(Match('r'))
                //{
                //  AddToken(TokenType.OR);
                //}
                //break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        // Raziel: Double Check this Reference.
                        CSLOX.Program.Error(_line, "Unexpected Character.");
                    }
                    break;
            }
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType tokenType, object? literal = null)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(tokenType, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (_source.ElementAt(_current) != expected)
            {
                return false;
            }
            _current++;
            return true;
        }

        // Peek is called Lookahead.
        // Usually the Lookahead is done one character at the time.
        // This will warranty speed on the scaner.
        // The more Characters ahead the slower is the scnner.
        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return _source.ElementAt(_current);
        }

        private void AddString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }

                Advance();
            }

            if (IsAtEnd())
            {
                CSLOX.Program.Error(_line, "Unterminated String.");
                return;
            }

            // Moving from the closing ".
            Advance();

            // Hint: If we want to support escape sequences
            // This is the place to do it.
            // Or if we want to use the $"{}" Style, it should be here to.

            // Get the values inside the "" only. That is why we do +1 and -1.
            string stringValue = _source.Substring(_start + 1, (_current - _start) - 1);
            AddToken(TokenType.STRING, stringValue);
        }

        private bool IsDigit(char character)
        {
            return character >= '0' && character <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the ".".
                Advance();
            }

            while (IsDigit(Peek()))
            {
                Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
            {
                return '\0';
            }

            return _source.ElementAt(_current + 1);
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            string text = _source.Substring(_start, _current - _start);
            TokenType? tokenType = _keywords[text];
            if (tokenType == null)
            {
                tokenType = TokenType.IDENTIFIER;
            }

            AddToken(tokenType.Value);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') && c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
