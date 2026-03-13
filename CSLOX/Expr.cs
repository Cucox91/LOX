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
            public T VisitAssingExpr(Assing expr);
            public T VisitGetExpr(Get expr);
            public T VisitSetExpr(Set expr);
            public T VisitLogicalExpr(Logical expr);
            public T VisitCallExpr(Call expr);
            public T VisitThisExpr(This expr);
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
            public object? Value { get; set; }
            public Literal(object? value)
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
            public Token? Name { get; set; }
            public Variable(Token? name)
            {
                  Name = name;
            }
            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitVariableExpr(this);
            }
      }

      public record Assing : Expr
      {
            public Token? Name { get; set; }
            public Expr? Value { get; set; }

            public Assing(Token? name, Expr? value)
            {
                  Name = name;
                  Value = value;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitAssingExpr(this);
            }
      }

      public record Logical : Expr
      {
            public Expr? Left { get; set; }
            public Token? Operator { get; set; }
            public Expr? Right { get; set; }

            public Logical(Expr? left, Token? oper, Expr? right)
            {
                  Left = left;
                  Operator = oper;
                  Right = right;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitLogicalExpr(this);
            }
      }

      public record Call : Expr
      {
            public Expr? Callee { get; set; }
            public Token? Paren { get; set; }

            public List<Expr?> Arguments { get; set; } = default!;


            public Call(Expr? callee, Token? paren, List<Expr?> arguments)
            {
                  Callee = callee;
                  Paren = paren;
                  Arguments = arguments;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitCallExpr(this);
            }
      }

      public record Get : Expr
      {
            public Expr? ExprObject { get; set; }
            public Token? Name { get; set; }

            public Get(Expr? exprObj, Token? name)
            {
                  Name = name;
                  ExprObject = exprObj;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitGetExpr(this);
            }
      }

      public record Set : Expr
      {
            public Expr? ExprObject { get; set; }
            public Token? Name { get; set; }
            public Expr? Value { get; set; }

            public Set(Expr? exprObject, Token? name, Expr? value)
            {
                  ExprObject = exprObject;
                  Name = name;
                  Value = value;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitSetExpr(this);
            }
      }

      public record This : Expr
      {
            public Token? Keyword { get; set; }

            public This(Token? keyword)
            {
                  Keyword = keyword;
            }

            public override Expr Accept<Expr>(IVisitorExpr<Expr> visitor)
            {
                  return visitor.VisitThisExpr(this);
            }
      }
}
