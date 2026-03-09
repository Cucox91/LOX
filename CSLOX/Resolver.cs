namespace CSLOX
{
    public class Resolver : IVisitorExpr<object>, IVisitorStmt<object>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION
        }

        private FunctionType _currentFuction = FunctionType.NONE;
        private Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();

        public Interpreter InterpreterProp { get; set; }
        public Resolver(Interpreter interpreter)
        {
            InterpreterProp = interpreter;
        }


        #region Useful Nodes for resolver...

        public object VisitBlockStmt(Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements!);
            EndScope();

            return null!;
        }

        public object VisitFunctionStmt(Function stmt)
        {
            Declare(stmt.Name!);
            Define(stmt.Name!);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null!;
        }

        public object VisitVarStmt(Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);

            return null!;
        }

        public object VisitVariableExpr(Variable expr)
        {
            if (_scopes.Count != 0 && _scopes.Peek()[expr.Name!.Lexeme] == false)
            {
                Program.Error(expr.Name.Line, "Can't Read Local variable in it's own initializer.");
            }

            ResolveLocal(expr, expr.Name!);

            return null!;
        }

        public object VisitAssingExpr(Assing expr)
        {
            Resolve(expr.Value!);
            ResolveLocal(expr, expr.Name!);
            return null!;
        }

        #endregion Useful Nodes for resolver...

        #region Resolver Specific Methods...

        public void Resolve(List<Stmt> statements)
        {
            foreach (var stmt in statements)
            {
                Resolve(stmt);
            }
        }

        private void ResolveFunction(Function func, FunctionType type)
        {
            FunctionType enclosingFunction = _currentFuction;
            _currentFuction = type;

            BeginScope();
            foreach (var param in func.Parameters)
            {
                Declare(param!);
                Define(param!);
            }
            Resolve(func.Body!);
            EndScope();

            _currentFuction = enclosingFunction;
        }

        private void ResolveLocal(Expr expr, Token token)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                InterpreterProp.Resolve(expr, _scopes.Count - 1 - i);
            }
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            Dictionary<string, bool> scope = _scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                Program.Error(name.Line, "Already a variable with this name on this scope.");
            }

            // The boolean value meants that if the variable have been resolved or not yet.
            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            var scope = _scopes.Peek();
            scope[name.Lexeme] = true;
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        #endregion Resolver Specific Methods...

        #region Useless Methods...

        public object VisitBinaryExpr(Binary expr)
        {
            Resolve(expr.Left!);
            Resolve(expr.Right!);
            return null!;
        }

        public object VisitCallExpr(Call expr)
        {
            Resolve(expr.Callee!);
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg!);
            }
            return null!;
        }

        public object VisitExpressionStmt(Expression stmt)
        {
            Resolve(stmt.Expre!);
            return null!;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            Resolve(expr.Expression!);
            return null!;
        }

        public object VisitIfStmt(If stmt)
        {
            Resolve(stmt.Condition!);
            Resolve(stmt.ThenBranch!);
            if (stmt.ElseBranch != null)
            {
                Resolve(stmt.ElseBranch);
            }
            return null!;
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return null!;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            Resolve(expr.Left!);
            Resolve(expr.Right!);
            return null!;
        }

        public object VisitPrintStmt(Print stmt)
        {
            Resolve(stmt.Expre!);
            return null!;
        }

        public object VisitReturnStmt(Return stmt)
        {
            if (_currentFuction == FunctionType.NONE)
            {
                Program.Error(stmt.Keyword!.Line, "Can't return from top-level code.");
            }

            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }
            return null!;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            Resolve(expr.Right!);
            return null!;
        }

        public object VisitWhileStmt(While stmt)
        {
            Resolve(stmt.Condition!);
            Resolve(stmt.Body!);
            return null!;
        }

        #endregion Useless Methods...
    }
}