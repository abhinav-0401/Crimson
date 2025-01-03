using Crimson.Lexing;
using Crimson.Parsing;

namespace Crimson.Evaluation;

internal static partial class Eval
{
    internal static void EvalProgram(List<Stmt> Program)
    {
        Value value = new NumValue(0);
        foreach (Stmt stmt in Program)
        {
            value = EvalStmt(stmt);
        }

        Console.WriteLine(value.ToString());
    }
}
