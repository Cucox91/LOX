using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using CSLOX;

if (args.Length != 1)
{
    Console.WriteLine("Use: generate_ast <output_directory>");
    Environment.Exit(64);
}

PrintExample();

static void PrintExample()
{
    Expr printExpt = new Binary(
       new Unary(
           new Token(TokenType.MINUS, "-", null, 1),
           new Literal(123)
       ),
       new Token(TokenType.STAR, "*", null, 1),
       new Grouping(
           new Literal("45.67")
       )
   );

    Console.WriteLine("Priting AST:");
    AstPrinter printer = new AstPrinter();
    Console.WriteLine(printer.Print(printExpt));

}
;