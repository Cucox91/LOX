namespace CSLOX
{
    public class LoxClass
    {
        public string Name { get; set; } = default!;

        public LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}