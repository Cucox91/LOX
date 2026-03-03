namespace CSLOX
{
    public class Interpreter : IVisitorExpr<object>, IVisitorStmt<object?>
    {
        private Environm _environment = new Environm();

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError er)
            {
                CSLOX.Program.RuntimeError(er);
            }
        }

        public object VisitBinaryExpr(Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            // Notice that below we are going to evaluate Left-To-Right.
            // This is very important here and we can't change that order.
            switch (expr.Oper.TokenType)
            {
                // Comparison
                case TokenType.GREATER:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left > (double)right;
                    }
                case TokenType.GREATER_EQUAL:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left >= (double)right;
                    }
                case TokenType.LESS:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left < (double)right;
                    }
                case TokenType.LESS_EQUAL:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left <= (double)right;
                    }
                case TokenType.BANG_EQUAL:
                    {
                        CheckNumberOperands(expr.Oper, left, right);

                        return !IsEaqual(left, right);
                    }
                case TokenType.EQUAL_EQUAL:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return IsEaqual(left, right);
                    }

                // Arithmetic
                case TokenType.MINUS:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left - (double)right;
                    }
                case TokenType.PLUS:
                    {
                        if (left is double l && right is double r)
                        {
                            return l + r;
                        }

                        if (left is string lft && right is string rgt)
                        {
                            return lft + rgt;
                        }

                        throw new RuntimeError(expr.Oper, "Operands must be two numbers or two strings.");
                    }
                case TokenType.SLASH:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left / (double)right;
                    }
                case TokenType.STAR:
                    {
                        CheckNumberOperands(expr.Oper, left, right);
                        return (double)left * (double)right;
                    }
            }

            return null!;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value!;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            var right = Evaluate(expr.Right);
            switch (expr.Oper.TokenType)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    {
                        CheckNumberOperand(expr.Oper, right);
                        return -(double)right;
                    }
            }

            return null!;
        }

        public object VisitVariableExpr(Variable expr)
        {
            // Raziel: Review nullability later on this.
            return _environment.Get(expr.Name!)!;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;

            if (obj is bool v) return v;

            return true;
        }

        private object Evaluate(Expr? expression)
        {
            return expression!.Accept(this);
        }

        private bool IsEaqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        private void CheckNumberOperand(Token token, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeError(token, "Operand must be a Number");
        }

        private void CheckNumberOperands(Token token, object left, object right)
        {
            if (left is double && right is double)
            {
                return;
            }

            throw new RuntimeError(token, "Operands must be a Number");
        }

        private string Stringify(object obj)
        {
            if (obj is null)
            {
                return "nil";
            }

            if (obj is double)
            {
                var text = obj.ToString() ?? "";
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString() ?? "";
        }

        // Statements Methods

        public object? VisitExpressionStmt(Expression stmt)
        {
            Evaluate(stmt.Expre);
            return null;
        }

        public object? VisitPrintStmt(Print stmt)
        {
            object val = Evaluate(stmt.Expre);
            Console.WriteLine(Stringify(val));
            return null;
        }

        public object? VisitVarStmt(Var stmt)
        {
            // Raziel: Work in Progress.
            object? val = null;
            if (stmt.Initializer != null)
            {
                val = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, val);
            return null;
        }

        public object VisitAssingExpr(Assing expr)
        {
            object val = Evaluate(expr.Value);
            _environment.Assign(expr.Name, val);
            return val;
        }

        public object? VisitBlockStmt(Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environm(_environment));
            return null;
        }

        private void ExecuteBlock(List<Stmt?> statements, Environm environm)
        {
            Environm previous = this._environment;
            try
            {
                this._environment = environm;
                foreach (var stmt in statements)
                {
                    Execute(stmt!);
                }
            }
            finally
            {
                this._environment = previous;
            }
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public object? VisitIfStmt(If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch!);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }

            return null;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Operator!.TokenType == TokenType.OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
        }

        #region Runtime Error Class.

        public class RuntimeError : Exception
        {
            public Token? Token { get; set; }

            public RuntimeError(Token token, string message) : base(message)
            {
                Token = token;
            }
        }

        #endregion Runtime Error Class.
    }
}