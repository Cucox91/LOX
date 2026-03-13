namespace CSLOX
{
    public class LoxClass : ILoxCallable
    {
        public string Name { get; set; } = default!;
        public Dictionary<string, LoxFunction> Methods { get; set; } = new Dictionary<string, LoxFunction>();

        public LoxClass(string name)
        {
            Name = name;
        }

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            Methods = methods;
        }

        public override string ToString()
        {
            return Name;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);

            LoxFunction initializer = FindMethod("init")!;
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public int Arity()
        {
            LoxFunction initializer = FindMethod("init")!;
            if (initializer == null)
            {
                return 0;
            }

            return initializer.Arity();
        }

        public LoxFunction? FindMethod(string lexeme)
        {
            Methods.TryGetValue(lexeme, out LoxFunction? val);
            return val;
        }
    }
}