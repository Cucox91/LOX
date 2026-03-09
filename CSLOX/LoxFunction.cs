using CSLOX;

public class LoxFunction : ILoxCallable
{

    private Function? _declaration;
    private Environm? _closure;

    public LoxFunction(Function declaration, Environm closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public int Arity()
    {
        return _declaration!.Parameters.Count;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Environm environment = new Environm(_closure!);
        for (int i = 0; i < _declaration!.Parameters.Count; i++)
        {
            environment.Define(_declaration!.Parameters[i]!.Lexeme, arguments[i]);
        }

        // We do this to avoid moving back all the levels of recursion.
        // We just simply throw an exception returning the resulting value.
        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (ReturnException ex)
        {
            return ex.Value!;
        }

        return null!;
    }

    public override string ToString()
    {
        return $"<fn {_declaration!.Name!.Lexeme}>";
    }
}