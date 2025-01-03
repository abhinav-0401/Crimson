using Crimson.Lexing;

namespace Crimson.Parsing;

internal partial class Parser
{
    // Method for Pratt Parsing Expressions
    internal Expr ParseExpr(int bindingPower)
    {
        if (_prefixes.TryGetValue(_currToken.Kind, out PrefixParselet prefixParselet))
        {
            Expr left = prefixParselet((int)BindingPower.Prefix);

            while (Peek().Kind != TokenKind.Eof && Peek().Kind != TokenKind.Semicolon && bindingPower < PeekBindingPower())
            {
                Advance();
                if (_infixes.TryGetValue(_currToken.Kind, out InfixParselet infixParselet))
                    left = infixParselet(left, PeekBindingPower());
            }

            return left;
        }

        return null;
    }

    private int PeekBindingPower()
    {
        if (_infixBindingPower.TryGetValue(Peek().Kind, out int bp))
            return bp;
        return (int)BindingPower.None;
    }

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

    internal Expr ParseBoolLitExpr(int bindingPower)
    {
        string literal = _currToken.Literal;

        if (literal == "true")
        {
            return new BoolLitExpr(true, _currToken);
        }
        else
        {
            return new BoolLitExpr(false, _currToken);
        }

    }

    internal Expr ParseNilLitExpr(int bindingPower)
    {
        return new NilLitExpr();
    }

    internal Expr ParsePrefixExpr(int bindingPower)
    {
        TokenKind op = _currToken.Kind;
        Advance();
        Expr expr = ParseExpr(bindingPower);
        return new PrefixExpr(op, expr);
    }

    internal Expr ParseGroupingExpr(int bindingPower)
    {
        Advance();
        Expr expr = ParseExpr((int)BindingPower.None);
        Advance();
        Match(TokenKind.RParen, "");
        return new GroupingExpr(expr);
    }

    internal Expr ParseBinaryExpr(Expr left, int bindingPower)
    {
        TokenKind op = _currToken.Kind;
        Advance();
        Expr right = ParseExpr(bindingPower);
        return new BinaryExpr(op, left, right);
    }
}