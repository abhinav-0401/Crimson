using Crimson.Lexing;
using Crimson.Parsing;

namespace Crimson;

internal class Program
{
    static void Main(string[] args)
    {
        //Lexer lexer = new("-45.5f* +-fagan number");
        Lexer lexer = new("6 + 2 * 3; 9 * 2;");
        var tokens = lexer.LexTokens();

        Parser parser = new(tokens);
        parser.Parse();
        
        foreach (var stmt in parser.Program)
        {
            Console.WriteLine(stmt.ToString());
        }
    }
}
