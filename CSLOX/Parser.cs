using System.Linq.Expressions;

namespace CSLOX
{
    public class Parser
    {
        public List<Token> Tokens { get; set; } = [];
        public int Current { get; set; }

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

        public Expr? Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }

        private Expr? Expression()
        {
            return Equality();
        }

        private Expr? Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token oper = Previous();
                Expr? right = Comparison();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private bool Match(params List<TokenType> tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().TokenType == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                Current++;
            }

            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().TokenType == TokenType.EOF;
        }

        private Token Peek()
        {
            return Tokens[Current];
        }

        private Token Previous()
        {
            return Tokens[Current - 1];
        }

        private Expr? Comparison()
        {
            Expr? expr = Term();
            while (
                Match(
                    TokenType.GREATER,
                    TokenType.GREATER_EQUAL,
                    TokenType.LESS,
                    TokenType.LESS_EQUAL
                )
            )
            {
                Token oper = Previous();
                Expr? right = Term();
                expr = new Binary(expr, oper, right);
            }
            return expr;
        }

        private Expr? Term()
        {
            Expr? expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token oper = Previous();
                Expr? right = Factor();
                expr = new Binary(expr, oper, right);
            }

            return expr;
        }

        private Expr? Factor()
        {
            Expr? expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token oper = Previous();
                Expr? right = Unary();
                expr = new Binary(expr, oper, right);
            }
            return expr;
        }

        private Expr? Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token oper = Previous();
                Expr? right = Unary();
                return new Unary(oper, right);
            }

            return Primary();
        }

        private Expr? Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr? expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after Expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect Expression");
        }

        private Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Program.Error(token.Line, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().TokenType == TokenType.SEMICOLON)
                {
                    return;
                }

                switch (Peek().TokenType)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }

        #region ParseError Class...

        private class ParseError : Exception
        {

        }

        #endregion  ParseError Class...
    }
}
