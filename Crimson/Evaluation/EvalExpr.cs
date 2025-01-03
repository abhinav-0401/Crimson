using Crimson.Lexing;
using Crimson.Parsing;

namespace Crimson.Evaluation;

internal static partial class Eval
{
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
            TokenKind.Plus => new NumValue(valNum.Value),
            TokenKind.Minus => new NumValue(-valNum.Value),
            TokenKind.Bang => BooleanUnaryOperation(expr.Op, val),
            _ => new NilValue(),
        };
    }

    private static Value EvalBinaryExpr(BinaryExpr expr)
    {
        Value leftValue = EvalExpr(expr.Left);
        Value rightValue = EvalExpr(expr.Right);

        return expr.Op switch
        {
            TokenKind.Plus or 
            TokenKind.Minus or 
            TokenKind.Star or 
            TokenKind.Slash => NumericalBinaryOperation(expr.Op, leftValue, rightValue),
            TokenKind.Gt or
            TokenKind.GtEq or
            TokenKind.Lt or 
            TokenKind.LtEq or 
            TokenKind.EqEq or
            TokenKind.BangEq => BooleanBinaryOperation(expr.Op, leftValue, rightValue),
            _ => new NilValue(),
        };
    }

    private static BoolValue BooleanUnaryOperation(TokenKind op, Value value)
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

    private static Value NumericalBinaryOperation(TokenKind op, Value leftValue, Value rightValue)
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
            TokenKind.Plus => new NumValue(leftValueNum.Value + rightValueNum.Value),
            TokenKind.Minus => new NumValue(leftValueNum.Value - rightValueNum.Value),
            TokenKind.Star => new NumValue(leftValueNum.Value * rightValueNum.Value),
            _ => new NilValue(),
        };
    }

    private static Value BooleanBinaryOperation(TokenKind op, Value leftValue, Value rightValue)
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
            TokenKind.Gt => new BoolValue(leftValueNum.Value > rightValueNum.Value),
            TokenKind.GtEq => new BoolValue(leftValueNum.Value >= rightValueNum.Value),
            TokenKind.Lt => new BoolValue(leftValueNum.Value < rightValueNum.Value),
            TokenKind.LtEq => new BoolValue(leftValueNum.Value <= rightValueNum.Value),
            TokenKind.EqEq => new BoolValue(leftValueNum.Value == rightValueNum.Value),
            TokenKind.BangEq => new BoolValue(leftValueNum.Value != rightValueNum.Value),
            _ => new NilValue(),
        };
    }
}