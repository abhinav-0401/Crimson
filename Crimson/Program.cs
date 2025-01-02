using Crimson.Lexing;
using Crimson.Parsing;
using Crimson.Evaluation;

namespace Crimson;

internal class Program
{
    static void Main(string[] args)
    {
        //Lexer lexer = new("-45.5f* +-fagan number");
        Lexer lexer = new("-6 + 2 * 3;");
        var tokens = lexer.LexTokens();

        Parser parser = new(tokens);
        parser.Parse();

        Eval.EvalProgram(parser.Program);
    }
}
