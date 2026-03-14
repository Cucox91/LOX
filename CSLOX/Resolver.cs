namespace CSLOX
{
    public class Resolver : IVisitorExpr<object>, IVisitorStmt<object>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION,
            METHOD,
            INITIALIZER
        }
        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }


        private FunctionType _currentFuction = FunctionType.NONE;
        private ClassType _currentClass = ClassType.NONE;

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
            if (_scopes.Count != 0 && _scopes.Peek().ContainsKey(expr.Name!.Lexeme) && _scopes.Peek()[expr.Name!.Lexeme] == false)
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
                if (_currentFuction == FunctionType.INITIALIZER)
                {
                    Program.Error(stmt.Keyword!, "Can't return value from an Initializer");
                }
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

        public object VisitClassStmt(Class stmt)
        {
            ClassType enclosingClass = _currentClass;
            _currentClass = ClassType.CLASS;

            Declare(stmt.Name!);
            Define(stmt.Name!);

            if (stmt.SuperClass != null && stmt.Name!.Lexeme == stmt.SuperClass.Name!.Lexeme)
            {
                Program.Error(stmt.SuperClass.Name, "A class can't inherit from itself.");
            }

            if (stmt.SuperClass != null)
            {
                _currentClass = ClassType.SUBCLASS;
                Resolve(stmt.SuperClass);
            }

            if (stmt.SuperClass != null)
            {
                BeginScope();
                Resolve(stmt.SuperClass);
            }

            BeginScope();

            _scopes.Peek().Add("this", true);
            foreach (var item in stmt.Methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (item.Name!.Lexeme == "init")
                {
                    declaration = FunctionType.INITIALIZER;
                }
                ResolveFunction(item, declaration);
            }


            EndScope();

            if (stmt.SuperClass != null)
            {
                EndScope();
            }

            _currentClass = enclosingClass;
            return null!;
        }

        public object VisitGetExpr(Get expr)
        {
            Resolve(expr.ExprObject!);
            return null!;
        }

        public object VisitSetExpr(Set expr)
        {
            Resolve(expr.ExprObject!);
            Resolve(expr.Value!);
            return null!;
        }

        public object VisitThisExpr(This expr)
        {
            if (_currentClass == ClassType.NONE)
            {
                Program.Error(expr.Keyword!, "Can't use 'this' outside a class.");
            }

            ResolveLocal(expr, expr.Keyword!);
            return null!;
        }

        public object VisitSuperExpr(Super expr)
        {
            if (_currentClass == ClassType.NONE)
            {
                Program.Error(expr.Keyword!, "Can't use 'super' outside a class.");
            }
            else if (_currentClass != ClassType.SUBCLASS)
            {
                Program.Error(expr.Keyword!, "Can't use 'super' in a class without a superclass.");
            }

            ResolveLocal(expr, expr.Keyword!);
            return null!;
        }
    }
}