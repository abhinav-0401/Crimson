using Crimson.Lexing;
using Crimson.Parsing;
using Crimson.Evaluation;

namespace Crimson;

internal class Program
{
    static void Main(string[] args)
    {
        Lexer lexer = new("print_ast (2 + 3) * 4; print 4 * (2 + 3) * 4;");
        var tokens = lexer.LexTokens();

        Parser parser = new(tokens);
        parser.Parse();

        Eval.EvalProgram(parser.Program);
    }
}
