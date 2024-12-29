using Crimson.Lexing;

namespace Crimson.Parsing;

internal partial class Parser
{
    internal Expr ParseNumLitExpr(int bindingPower)
    {
        string literal = _currToken.Literal;
        
        if (double.TryParse(literal, out double doubleLiteral))
        {
            return new NumLitExpr(doubleLiteral, _currToken);
        }

        Console.WriteLine("Could not parse {0} as number (double) literal", literal);
        Environment.Exit(0);
        return null;
    }

    internal Expr ParsePrefixExpr(int bindingPower)
    {
        TokenKind op = _currToken.Kind;
        Advance();
        Expr expr = ParseExpr(bindingPower);
        return new PrefixExpr(op, expr);
    }

    internal Expr ParseBinaryExpr(Expr left, int bindingPower)
    {
        TokenKind op = _currToken.Kind;
        Advance();
        Expr right = ParseExpr(bindingPower);
        return new BinaryExpr(op, left, right);
    }
}