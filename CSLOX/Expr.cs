namespace CSLOX
{
      public abstract record Expr
      {
            public abstract Expr Accept<Expr>(IVisitorExpr<Expr> visitor);
      }

      public interface IVisitorExpr<T>
      {
            public T VisitBinaryExpr(Binary expr);
            public T VisitGroupingExpr(Grouping expr);
            public T VisitLiteralExpr(Literal expr);
            public T VisitUnaryExpr(Unary expr);
            public T VisitVariableExpr(Variable expr);
      }

      public record Binary : Expr
      {
            public Expr? Left { get; set; }
            public Token Oper { get; set; }
            public Expr? Right { get; set; }
            public Binary(Expr? left, Token oper, Expr? right)
            {
                  Left = left;
                  Oper = oper;
                  Right = right;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            { return visitor.VisitBinaryExpr(this); }
      }
      public record Grouping : Expr
      {
            public Expr? Expression { get; set; }
            public Grouping(Expr? expression)
            {
                  Expression = expression;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            { return visitor.VisitGroupingExpr(this); }
      }
      public record Literal : Expr
      {
            public Object? Value { get; set; }
            public Literal(Object? value)
            {
                  Value = value;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            { return visitor.VisitLiteralExpr(this); }
      }
      public record Unary : Expr
      {
            public Token Oper { get; set; }
            public Expr? Right { get; set; }
            public Unary(Token oper, Expr? right)
            {
                  Oper = oper;
                  Right = right;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            { return visitor.VisitUnaryExpr(this); }
      }

      public record Variable : Expr
      {
            public Object? Name { get; set; }
            public Variable(Object? name)
            {
                  Name = name;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitVariableExpr(this);
            }
      }

}
