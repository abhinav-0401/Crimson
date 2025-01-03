using Crimson.Parsing;

namespace Crimson.Evaluation;

internal static class Eval
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
            BoolLitExpr boolLitExpr => new BoolValue(boolLitExpr.Value),
            PrefixExpr prefixExpr => EvalPrefixExpr(prefixExpr),
            BinaryExpr binaryExpr => EvalBinaryExpr(binaryExpr),
            GroupingExpr groupingExpr => EvalExpr(groupingExpr.Expression),
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
        return expr.Op switch
        {
            Lexing.TokenKind.Plus => new NumValue(valNum.Value),
            Lexing.TokenKind.Minus => new NumValue(-valNum.Value),
            Lexing.TokenKind.Bang => BooleanUnaryOperation(expr.Op, val),
            _ => new NumValue(0),
        };
    }

    private static Value EvalBinaryExpr(BinaryExpr expr)
    {
        Value leftValue = EvalExpr(expr.Left);
        Value rightValue = EvalExpr(expr.Right);

        return expr.Op switch
        {
            Lexing.TokenKind.Plus or 
            Lexing.TokenKind.Minus or 
            Lexing.TokenKind.Star or 
            Lexing.TokenKind.Slash => NumericalBinaryOperation(expr.Op, leftValue, rightValue),
            Lexing.TokenKind.Gt or
            Lexing.TokenKind.GtEq or
            Lexing.TokenKind.Lt or 
            Lexing.TokenKind.LtEq or 
            Lexing.TokenKind.EqEq or
            Lexing.TokenKind.BangEq => BooleanBinaryOperation(expr.Op, leftValue, rightValue),
            _ => NumericalBinaryOperation(expr.Op, leftValue, rightValue),
        };
    }

    private static BoolValue BooleanUnaryOperation(Lexing.TokenKind op, Value value)
    {
        if (value.ValueKind == ValueType.BoolType)
        {
            var valueBool = (BoolValue)value;
            return new BoolValue(!valueBool.Value);
        }
        else if (value.ValueKind == ValueType.NumType)
        {
            var valueNum = (NumValue)value;
            if (valueNum.Value > 0) { return new BoolValue(false); }
            else { return new BoolValue(true); }
        }
        
        return new BoolValue(false);
    }

    private static Value NumericalBinaryOperation(Lexing.TokenKind op, Value leftValue, Value rightValue)
    {
        if (leftValue.ValueKind != ValueType.NumType || rightValue.ValueKind != ValueType.NumType)
        {
            Console.WriteLine("{0} can only be performed on two number types", op);
            Environment.Exit(1);
        }

        var leftValueNum = (NumValue)leftValue;
        var rightValueNum = (NumValue)rightValue;
        return op switch
        {
            Lexing.TokenKind.Plus => new NumValue(leftValueNum.Value + rightValueNum.Value),
            Lexing.TokenKind.Minus => new NumValue(leftValueNum.Value - rightValueNum.Value),
            Lexing.TokenKind.Star => new NumValue(leftValueNum.Value * rightValueNum.Value),
            _ => new NumValue(0),
        };
    }

    private static Value BooleanBinaryOperation(Lexing.TokenKind op, Value leftValue, Value rightValue)
    {
        if (leftValue.ValueKind != ValueType.NumType || rightValue.ValueKind != ValueType.NumType)
        {
            Console.WriteLine("{0} can only be performed on two number types", op);
            Environment.Exit(1);
        }

        var leftValueNum = (NumValue)leftValue;
        var rightValueNum = (NumValue)rightValue;
        return op switch
        {
            Lexing.TokenKind.Gt => new BoolValue(leftValueNum.Value > rightValueNum.Value),
            Lexing.TokenKind.GtEq => new BoolValue(leftValueNum.Value >= rightValueNum.Value),
            Lexing.TokenKind.Lt => new BoolValue(leftValueNum.Value < rightValueNum.Value),
            Lexing.TokenKind.LtEq => new BoolValue(leftValueNum.Value <= rightValueNum.Value),
            Lexing.TokenKind.EqEq => new BoolValue(leftValueNum.Value == rightValueNum.Value),
            Lexing.TokenKind.BangEq => new BoolValue(leftValueNum.Value != rightValueNum.Value),
            _ => new BoolValue(false),
        };
    }
}
