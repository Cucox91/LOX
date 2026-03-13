namespace CSLOX
{
    public class Interpreter : IVisitorExpr<object>, IVisitorStmt<object?>
    {
        public Environm Globals { get; set; } = new Environm()!;
        private Environm _environment = default!;
        private Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();

        public Interpreter()
        {
            _environment = Globals; // Raziel: check this, the assignemnt of global to envs. 
            Globals.Define("Clock", new LoxCallableClock());
        }

        #region Expressions Methods...

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
            // return _environment.Get(expr.Name!)!;
            return LookupVariable(expr.Name!, expr)!;
        }

        public object VisitCallExpr(Call expr)
        {
            object callee = Evaluate(expr.Callee);

            List<object> arguments = new List<object>();
            foreach (var arg in expr.Arguments)
            {
                arguments.Add(Evaluate(arg));
            }

            if (callee is not ILoxCallable)
            {
                throw new RuntimeError(expr.Paren!, "Can only call Functions and Classes.");
            }

            var function = (ILoxCallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren!, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        #endregion Expressions Methods...

        #region Auxiliary Methods...

        private object LookupVariable(Token name, Expr expr)
        {
            if (_locals.TryGetValue(expr, out int distance))
            {
                // Raziel: Verify this for global.
                var result = _environment.GetAt(distance, name.Lexeme)!;
                if (result != null)
                {
                    return result;
                }
            }

            return Globals.Get(name)!;
        }

        public void Resolve(Expr expr, int depth)
        {
            //Check if this break the recursions.
            if (!_locals.TryAdd(expr, depth))
            {
                _locals[expr] = depth;
            }
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    // Adding this if to remove the executon of null statements generated by whitespaces.
                    if (statement != null)
                    {
                        Execute(statement);
                    }
                }
            }
            catch (RuntimeError er)
            {
                CSLOX.Program.RuntimeError(er);
            }
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

        #endregion Auxiliary Methods...

        #region Statements Methods...

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
            if (_locals.TryGetValue(expr, out int distance))
            {
                _environment.AssignAt(distance, expr.Name!, val);
            }
            else
            {
                Globals.Assign(expr.Name, val);
            }
            return val;
        }

        public object? VisitBlockStmt(Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environm(_environment));
            return null;
        }

        public void ExecuteBlock(List<Stmt?> statements, Environm environm)
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

        public object? VisitWhileStmt(While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body!);
            }
            return null;
        }

        public object? VisitFunctionStmt(Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, _environment);
            _environment.Define(stmt!.Name!.Lexeme, function);
            return null;
        }

        public object? VisitReturnStmt(Return stmt)
        {
            object? value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            throw new ReturnException(value);
        }

        public object? VisitClassStmt(Class stmt)
        {
            _environment.Define(stmt.Name!.Lexeme, null);
            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach (var item in stmt.Methods)
            {
                LoxFunction func = new LoxFunction(item, _environment, item.Name!.Lexeme == "init");
                methods.Add(item.Name!.Lexeme, func);
            }

            LoxClass klass = new LoxClass(stmt.Name!.Lexeme, methods);
            _environment.Assign(stmt.Name, klass);
            return null;
        }

        public object VisitGetExpr(Get expr)
        {
            object obj = Evaluate(expr.ExprObject!);
            if (obj is LoxInstance)
            {
                return (obj as LoxInstance)!.Get(expr.Name!);
            }

            throw new RuntimeError(expr.Name!, "Only instances have properties.");
        }

        public object VisitSetExpr(Set expr)
        {
            object obj = Evaluate(expr.ExprObject);

            if (obj is not LoxInstance)
            {
                throw new RuntimeError(expr.Name!, "Only instances have fields.");
            }

            object val = Evaluate(expr.Value);
            ((LoxInstance)obj).Set(expr.Name!, val);
            return val;
        }

        public object VisitThisExpr(This expr)
        {
            return LookupVariable(expr.Keyword!, expr);
        }

        #endregion Statements Methods...

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

        #region LoxCallableClock...

        public class LoxCallableClock : ILoxCallable
        {

            public int Arity()
            {
                return 0;
            }

            public object Call(Interpreter interpreter, List<object> arguments)
            {
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            public override string ToString()
            {
                return "<native fn>";
            }
        }

        #endregion LoxCallableClock...
    }
}