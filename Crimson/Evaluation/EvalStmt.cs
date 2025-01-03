using Crimson.Lexing;
using Crimson.Parsing;

namespace Crimson.Evaluation;

internal static partial class Eval
{
    private static Value EvalStmt(Stmt stmt)
    {
        return stmt switch
        {
            ExprStmt exprStmt => EvalExprStmt(exprStmt),
            PrintStmt printStmt => EvalPrintStmt(printStmt),
            PrintAstStmt printAstStmt => EvalPrintAstStmt(printAstStmt),
            _ => new NumValue(1),
        };
    }

    private static Value EvalExprStmt(ExprStmt exprStmt)
    {
        EvalExpr(exprStmt.Expression);
        return new NilValue();
    }

    private static Value EvalPrintStmt(PrintStmt printStmt)
    {
        Value value = EvalExpr(printStmt.Expression);
        Console.WriteLine(value);
        return new NilValue();
    }

    private static Value EvalPrintAstStmt(PrintAstStmt printAstStmt)
    {
        Console.WriteLine(printAstStmt.Expression.ToString());
        return new NilValue();
    }
}