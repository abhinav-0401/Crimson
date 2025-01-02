using Crimson.Parsing;

namespace Crimson.Evaluation;

internal class Eval
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

    private static Value EvalStmt(Stmt stmt)
    {
        return stmt switch
        {
            ExprStmt exprStmt => EvalExprStmt(exprStmt),
            _ => new NumValue(1),
        };
    }

    private static Value EvalExprStmt(ExprStmt exprStmt)
    {
        Console.WriteLine(exprStmt.Expression.ToString());
        return EvalExpr(exprStmt.Expression);
    }

    private static Value EvalExpr(Expr expr)
    {
        return expr switch
        {
            NumLitExpr numLitExpr => new NumValue(numLitExpr.Value),
            PrefixExpr prefixExpr => EvalPrefixExpr(prefixExpr),
            BinaryExpr binaryExpr => EvalBinaryExpr(binaryExpr),
            _ => new NumValue(-100),
        };
    }

    private static Value EvalPrefixExpr(PrefixExpr expr)
    {
        Value val = EvalExpr(expr.Expression);
        
        if (val.ValueKind != ValueType.NumType)
        {
            Console.WriteLine("Prefix operators must take only numerical expressions as arguments");
            Environment.Exit(1);
        }

        var valNum = (NumValue)val;
        switch (expr.Op)
        {
            case Lexing.TokenKind.Plus:
                return new NumValue(valNum.Value);
            case Lexing.TokenKind.Minus:
                return new NumValue(-valNum.Value);
            default:
                return new NumValue(0);

        }
    }

    private static Value EvalBinaryExpr(BinaryExpr expr)
    {
        Value leftValue = EvalExpr(expr.Left);
        Value rightValue = EvalExpr(expr.Right);

        switch (expr.Op)
        {
            case Lexing.TokenKind.Plus:
                return BinaryOperation(expr.Op, leftValue, rightValue);
            case Lexing.TokenKind.Minus:
                return BinaryOperation(expr.Op, leftValue, rightValue);
            case Lexing.TokenKind.Star:
                return BinaryOperation(expr.Op, leftValue, rightValue);
            case Lexing.TokenKind.Slash:
                return BinaryOperation(expr.Op, leftValue, rightValue);
            default:
                return BinaryOperation(expr.Op, leftValue, rightValue);
        }
    }

    private static Value BinaryOperation(Lexing.TokenKind op, Value leftValue, Value rightValue)
    {
        if (leftValue.ValueKind != ValueType.NumType || rightValue.ValueKind != ValueType.NumType)
        {
            Console.WriteLine("{0} can only be performed on two number types", op);
            Environment.Exit(1);
        }

        var leftValueNum = (NumValue)leftValue;
        var rightValueNum = (NumValue)rightValue;
        switch (op)
        {
            case Lexing.TokenKind.Plus:
                return new NumValue(leftValueNum.Value + rightValueNum.Value);
            case Lexing.TokenKind.Minus:
                return new NumValue(leftValueNum.Value - rightValueNum.Value);
            case Lexing.TokenKind.Star:
                return new NumValue(leftValueNum.Value * rightValueNum.Value);
            default:
                return new NumValue(0);
        }
    }
}
