using System.Buffers;
using System.Text;

namespace CSLOX
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parentesis(expr.Oper.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parentesis("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr == null || expr.Value == null)
            {
                return "nil";
            }
            return expr.Value.ToString() ?? "";
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parentesis(expr.Oper.Lexeme, expr.Right);
        }

        private string Parentesis(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}