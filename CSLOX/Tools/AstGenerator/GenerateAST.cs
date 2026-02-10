// using System.Globalization;
// using System.Reflection.Metadata;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Text.Unicode;

// if (args.Length != 1)
// {
//     Console.WriteLine("Use: generate_ast <output_directory>");
//     Environment.Exit(64);
// }

// string output_directory = args[0];

// DefineAst(output_directory, "Expr",
//     [
//         "Binary   : Expr left, Token oper, Expr right",
//         "Grouping : Expr expression",
//         "Literal  : Object value",
//         "Unary    : Token oper, Expr right",
//     ]);

// static void DefineAst(string outputDirectory, string baseName, List<string> types)
// {
//     try
//     {
//         string path = $"{outputDirectory}/{baseName}.cs";
//         using StreamWriter writer = new(path);

//         // Header Comment for the generated code to format for better readability.
//         writer.WriteLine("// Just Apply an autoformatter. (CTRL + SHIFT + I) in VS Code Linux.");

//         // This will define the Namespace plus the Parent Class.
//         writer.WriteLine("namespace CSLOX {");
//         writer.WriteLine($" public abstract record {baseName} {{");

//         writer.WriteLine($"public abstract {baseName} Accept<{baseName}>(IVisitor<{baseName}> visitor);");

//         writer.WriteLine("}"); //Closing Brackets for abstract base class.

//         // This will define the Visitor interface with the methods inside. 
//         DefineVisitor(writer, baseName, types);

//         // This will define the child classes per type inside the parent class
//         foreach (var type in types)
//         {
//             string[] typeSplit = type.Split(":");
//             string className = typeSplit[0].Trim();
//             string fields = typeSplit[1].Trim();
//             DefineType(writer, baseName, className, fields);
//         }

//         writer.WriteLine("}");  //Closing Brackets for namespace.

//         writer.Close();
//     }
//     catch (System.Exception ex)
//     {
//         Console.Error.WriteLine("Error trying to generate the AST Files");
//         Console.WriteLine(ex.Message);
//     }
// }

// static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
// {
//     string[] fields = fieldList.Split(", ");

//     // Creating Sub-Class.
//     writer.WriteLine($"public record {className}: {baseName} {{");

//     // Creating each property.
//     foreach (var field in fields)
//     {
//         string[] fieldSplit = field.Split(" ");
//         writer.WriteLine($"public {fieldSplit[0]} {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldSplit[1])} {{get; set;}}");
//     }

//     // Creating the constructor for the class with parameters.
//     writer.WriteLine($"     {className}({fieldList}) {{");

//     // Assigning parameters to fields.
//     foreach (var field in fields)
//     {
//         string[] fieldSplit = field.Split(" ");
//         writer.WriteLine($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldSplit[1])} = {fieldSplit[1]};");
//     }

//     writer.WriteLine("      }");    // Closing bracket for constructor.

//     // Adding the Accept overrride method to the class.
//     writer.WriteLine($"public override {baseName} Accept<{baseName}>(IVisitor<{baseName}> visitor)");
//     writer.WriteLine($"{{ return visitor.Visit{className}{baseName}(this); }}");

//     writer.WriteLine("      }");    // Closing bracket for class.
// }

// static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
// {
//     writer.WriteLine("public interface IVisitor<T> {");

//     foreach (var typeName in types)
//     {
//         string[] typeSplit = typeName.Split(":");
//         string className = typeSplit[0].Trim();
//         string fields = typeSplit[1].Trim();


//         writer.WriteLine($"public T Visit{className}{baseName}({className} {baseName.ToLower()});");
//     }

//     writer.WriteLine("}");
// }
