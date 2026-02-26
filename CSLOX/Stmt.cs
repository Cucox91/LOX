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
}