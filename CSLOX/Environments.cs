/******************************************************************************************
    If you need to know more about variable scope refer to the SCHEME Programming Language.
*******************************************************************************************/

using static CSLOX.Interpreter;

namespace CSLOX
{
    public class Environm
    {
        public Environm? Enclosing { get; set; }

        private Dictionary<string, object?> _values = new Dictionary<string, object?>();

        public Environm()
        {
            Enclosing = null;
        }

        public Environm(Environm enclosing)
        {
            Enclosing = enclosing;
        }

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

            if (Enclosing != null)
            {
                return Enclosing.Get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token? name, object val)
        {
            if (_values.ContainsKey(name!.Lexeme))
            {
                _values[name.Lexeme] = val;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, val);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}