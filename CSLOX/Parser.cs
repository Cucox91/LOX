using System.ComponentModel;
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

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration()!);
            }

            return statements;
        }

        private Stmt? Declaration()
        {
            try
            {
                if (Match(TokenType.VAR))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseError)
            {
                // We do nothing with the error above because this will  be synchronized and
                // we don't want to show internal errors. Just the high level ones.

                Synchronize();
                return null;
            }
        }

        private Expr? Expression()
        {
            return Assignment();
            // return Equality();
        }

        private Expr? Assignment()
        {
            // Expr expr = Equality()!;
            Expr expr = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equalss = Previous();
                Expr value = Assignment()!;

                if (expr is Variable)
                {
                    Token name = (expr as Variable)!.Name!;
                    return new Assing(name, value);
                }

                Error(equalss, "Invalid Assignment Target.");
            }

            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();
            while (Match(TokenType.OR))
            {
                Token oper = Previous();
                Expr right = And();
                expr = new Logical(expr, oper, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality()!;
            while (Match(TokenType.AND))
            {
                Token oper = Previous();
                Expr? right = Equality();
                expr = new Logical(expr, oper, right);
            }
            return expr;
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

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
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


        // Related to Statement:
        private Stmt Statement()
        {
            if (Match(TokenType.IF))
            {
                return IfStatement();
            }

            if (Match(TokenType.FOR))
            {
                return ForStatement();
            }

            if (Match(TokenType.PRINT))
            {
                return PrintStatement();
            }

            if (Match(TokenType.WHILE))
            {
                return WhileStatement();
            }

            if (Match(TokenType.LEFT_BRACE))
            {
                return new Block(Block()!);
            }

            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after the 'for'");

            // Initializer (P1)
            Stmt? initializer = null;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            // Condition (P2)
            Expr? condition = null; ;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expected ';' after loop condition.");

            // Increment (P3)
            Expr? increment = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                increment = Expression();
            }
            
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after loop increment.");

            // Body (P4)
            Stmt? body = Statement();

            // Here we are desugaring. This meant that we will pass this as a while loop that we already have to the Interpreter.
            // Here what we do is to Convert and build it. We are making the body the composition of the multiple parts.

            if (increment != null)
            {
                body = new Block(new List<Stmt?> { body, new Expression(increment) });
            }

            if (condition == null)
            {
                condition = new Literal(true);
            }
            body = new While(condition, body);

            if (initializer != null)
            {
                body = new Block(new List<Stmt?> { initializer, body });
            }

            return body;
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression()!;
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after 'while'.");
            Stmt body = Statement();
            return new While(condition, body);
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr? condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after 'if' condition");

            Stmt? thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new If(condition, thenBranch, elseBranch);
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Exected Variable Name");

            Expr? initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration.");

            return new Var(name, initializer!);
        }

        private Stmt PrintStatement()
        {
            Expr? value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Print(value!);
        }

        private Stmt ExpressionStatement()
        {
            Expr? value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Expression(value!);
        }

        private List<Stmt?> Block()
        {
            List<Stmt?> statements = new List<Stmt?>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expected '}' after block.");

            return statements;
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
