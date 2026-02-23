/******************************************************************************************
    If you need to know more about variable scope refer to the SCHEME Programming Language.
*******************************************************************************************/

using static CSLOX.Interpreter;

namespace CSLOX
{
    public class Environm
    {
        private Dictionary<string, object?> _values = new Dictionary<string, object?>();

        public void Define(string name, object? value)
        {
            _values.Add(name, value);
        }

        public object? Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}