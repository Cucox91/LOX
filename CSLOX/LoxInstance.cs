using static CSLOX.Interpreter;

namespace CSLOX
{
    public class LoxInstance
    {
        private LoxClass? _loxClass = null;

        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();

        public LoxInstance(LoxClass loxClass)
        {
            _loxClass = loxClass;
        }

        public object Get(Token name)
        {
            if (Fields.ContainsKey(name.Lexeme))
            {
                return Fields[name.Lexeme];
            }

            LoxFunction? method = _loxClass!.FindMethod(name.Lexeme);
            if (method != null)
            {
                return method.Bind(this);
            }

            throw new RuntimeError(name, $"Undefine property {name.Lexeme}.");
        }

        public object Set(Token name, object value)
        {
            Fields.Add(name.Lexeme, value);
            return null!;
        }

        public override string ToString()
        {
            return _loxClass!.Name + " instance";
        }
    }
}