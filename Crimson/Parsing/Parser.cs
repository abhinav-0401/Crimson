﻿using Crimson.Lexing;

namespace Crimson.Parsing;

internal enum BindingPower
{
    None = 0,
    Assignment = 1,  // =, +=, -=, etc.
    LogicalOr = 2,   // ||
    LogicalAnd = 3,  // &&
    Equality = 4,    // ==, !=
    Comparison = 5,  // <, >, <=, >=
    Sum = 6,         // +, -
    Product = 7,     // *, /, %
    Prefix = 8,
    Postfix = 9,
    Call = 10
}

internal partial class Parser
{
    private int _pos;
    private int _readPos;
    private Token _currToken;
    private List<Token> _tokens;

    private delegate Expr PrefixParselet(int bindingPower);
    private delegate Expr InfixParselet(Expr left, int bindingPower);

    private Dictionary<TokenKind, PrefixParselet> _prefixes;
    private Dictionary<TokenKind, InfixParselet> _infixes;
    private readonly Dictionary<TokenKind, int> _infixBindingPower = new()
    {
        { TokenKind.Plus, (int)BindingPower.Sum },
        { TokenKind.Minus, (int)BindingPower.Sum },
        { TokenKind.Star, (int)BindingPower.Product },
        { TokenKind.Slash, (int)BindingPower.Product },
        { TokenKind.EqEq, (int)BindingPower.Equality },
        { TokenKind.Lt, (int)BindingPower.Comparison },
        { TokenKind.Gt, (int)BindingPower.Comparison },
    };

    internal Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _pos = 0;
        _readPos = 1;
        _currToken = _tokens[0];

        _prefixes = new Dictionary<TokenKind, PrefixParselet>()
        {
            { TokenKind.NumLit, ParseNumLitExpr },
            { TokenKind.Bang, ParsePrefixExpr },
        };

        _infixes = new Dictionary<TokenKind, InfixParselet>()
        {
            { TokenKind.Plus, ParseBinaryExpr },
            { TokenKind.Minus, ParseBinaryExpr },
            { TokenKind.Star, ParseBinaryExpr },
            { TokenKind.Slash, ParseBinaryExpr },
        };
    }
    
    internal void Parse()
    {
        while (IsNotEof())
        {
            Console.WriteLine(ParseExpr((int)BindingPower.None)?.ToString());
        }
    }

    internal Expr ParseExpr(int bindingPower)
    {
        if (_prefixes.TryGetValue(_currToken.Kind, out PrefixParselet prefixParselet))
        {
            Expr left = prefixParselet((int)BindingPower.Prefix);
            Advance();
            
            if (_currToken.Kind == TokenKind.Eof)
            {
                if (left != null) { return left; }
            }

            while (_currToken.Kind != TokenKind.Eof && bindingPower < _infixBindingPower[_currToken.Kind])
            {
                if (_infixes.TryGetValue(_currToken.Kind, out InfixParselet infixParselet))
                {
                    left = infixParselet(left, _infixBindingPower[_currToken.Kind]);
                }     
            }

            return left;
        }

        return null;
    }

    private bool IsNotEof()
    {
        return _currToken.Kind != TokenKind.Eof;
    }

    private Token Advance()
    {
        _pos++;
        _readPos++;
        _currToken = _tokens[_pos];
        return _tokens[_pos];
    }

    private Token Peek()
    {
        return _tokens[_readPos];
    }

    private bool Match(TokenKind kind)
    {
        return Peek().Kind == kind;
    }

    private Token Expect(TokenKind kind, string msg)
    {
        if (Peek().Kind != kind)
        {
            if (msg == "" || msg == null)
            {
                msg = String.Format("Error: Expected token of type {0}, found token of type {1}\n", kind.ToString(), Peek().Kind.ToString());
            }
            Console.WriteLine(msg);
            Environment.Exit(1);
        }
        return Advance();
    }
}
