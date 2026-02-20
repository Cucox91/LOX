using System;
using System.Text;

namespace CSLOX
{
    public class Program
    {
        private static bool _hadErrors = false;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                if (args[0] == "Build Tree")
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
                    AstPrinter printer = new();
                    Console.WriteLine(printer.Print(printExpt));
                }
                else
                {
                    RunFile(args[0]);
                }
            }
            else
            {
                RunPrompt();
            }
        }

        /// <summary>
        /// Loading the Source from a File.
        /// </summary>
        /// <param name="path">The path to the File to load.</param>
        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            // I just will assume here that we are dealing with UTF8.
            var source = System.Text.Encoding.UTF8.GetString(bytes);
            Run(source);

            if (_hadErrors)
            {
                Environment.Exit(65);
            }
        }

        /// <summary>
        /// Interactive Mode in the CLI reading from parameters.
        /// This is usually called REPL.
        /// </summary>
        private static void RunPrompt()
        {
            while (true)
            {
                Console.WriteLine("> ");
                string? line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }
                else
                {
                    Run(line);
                    // This is to avoid killing the whole interactive terminal.
                    _hadErrors = false;
                }
            }
        }

        private static void Run(string source)
        {
            // Create List of tokens by using the scanner.
            Scanner scanner = new(source);
            List<Token> tokens = scanner.ScanTokens();

            // Take the created list of tokents and pass them to the Parser.
            Parser parser = new Parser(tokens);
            var expr = parser.Parse();

            if (_hadErrors || expr == null)
            {
                return;
            }

            Console.WriteLine(new AstPrinter().Print(expr!));
        }

        /// <summary>
        /// Generates an Error.
        /// This is the part that creates the error based on conditions.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (TokenType.EOF == token.TokenType)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        /// <summary>
        /// Reports an Error.
        /// This is the part that Shows the Error to the user.
        /// There are Different ways.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="where"></param>
        /// <param name="message"></param>
        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line: {line} ] Error: {where} : {message}");
        }
    }
}
