namespace CSLOX
{
    public abstract record Stmt
    {
        public abstract Expr Accept<Expr>(IVisitorStmt<Expr> visitor);
    }

    public interface IVisitorStmt<T>
    {
        public T VisitExpressionStmt(Expression stmt);
        public T VisitPrintStmt(Print stmt);
        public T VisitVarStmt(Var stmt);
        public T VisitBlockStmt(Block stmt);
        public T VisitIfStmt(If stmt);
        public T VisitWhileStmt(While stmt);
        public T VisitFunctionStmt(Function stmt);
        public T VisitReturnStmt(Return stmt);
        public T VisitClassStmt(Class stmt);
    }

    public record Expression : Stmt
    {
        public Expr? Expre { get; set; }

        public Expression(Expr expr)
        {
            Expre = expr;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public record Print : Stmt
    {
        public Expr? Expre { get; set; }

        public Print(Expr expr)
        {
            Expre = expr;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public record Var : Stmt
    {
        public Token Name { get; set; }

        public Expr? Initializer { get; set; }

        public Var(Token name, Expr? initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }

    public record Block : Stmt
    {
        public List<Stmt?> Statements { get; set; } = new List<Stmt?>();

        public Block(List<Stmt?> stmts)
        {
            Statements = stmts;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    public record If : Stmt
    {
        public Expr? Condition { get; set; } = default;
        public Stmt? ThenBranch { get; set; }
        public Stmt? ElseBranch { get; set; }

        public If(Expr? condition, Stmt? thenBranch, Stmt? elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }

    public record While : Stmt
    {
        public Expr? Condition { get; set; }
        public Stmt? Body { get; set; }

        public While(Expr? condition, Stmt? body)
        {
            Condition = condition;
            Body = body;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }

    public record Function : Stmt
    {
        public Token? Name { get; set; }
        public List<Token?> Parameters { get; set; } = default!;
        public List<Stmt?> Body { get; set; } = default!;

        public Function(Token? name, List<Token?> parameters, List<Stmt?> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }

    public record Return : Stmt
    {
        public Token? Keyword { get; set; }
        public Expr? Value { get; set; }

        public Return(Token? keywordParam, Expr? valueParam)
        {
            Keyword = keywordParam;
            Value = valueParam;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }

    public record Class : Stmt
    {
        public Token? Name { get; set; }
        public Variable? SuperClass { get; set; }
        public List<Function> Methods { get; set; } = new List<Function>();

        public Class(Token? name, Variable? superClass, List<Function> methods)
        {
            Name = name;
            SuperClass = superClass;
            Methods = methods;
        }

        public override Expr Accept<Expr>(IVisitorStmt<Expr> visitor)
        {
            return visitor.VisitClassStmt(this);
        }
    }
}