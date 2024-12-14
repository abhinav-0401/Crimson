using Crimson.Lexing;

namespace Crimson;

internal class Program
{
    static void Main(string[] args)
    {
        Lexer lexer = new("-45.5f* +-fagan number");
        lexer.LexTokens();
        
        foreach (var token in lexer.Tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }
}
