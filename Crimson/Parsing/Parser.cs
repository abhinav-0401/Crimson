using Crimson.Lexing;

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

    public List<Stmt> Program { get; private set; } = new();

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
            { TokenKind.True, ParseBoolLitExpr },
            { TokenKind.False, ParseBoolLitExpr },
            { TokenKind.Nil, ParseBoolLitExpr },
            { TokenKind.Plus, ParsePrefixExpr },
            { TokenKind.Minus, ParsePrefixExpr },
            { TokenKind.Bang, ParsePrefixExpr },
            { TokenKind.LParen, ParseGroupingExpr },
        };

        _infixes = new Dictionary<TokenKind, InfixParselet>()
        {
            { TokenKind.Plus, ParseBinaryExpr },
            { TokenKind.Minus, ParseBinaryExpr },
            { TokenKind.Star, ParseBinaryExpr },
            { TokenKind.Slash, ParseBinaryExpr },
            { TokenKind.Gt, ParseBinaryExpr },
            { TokenKind.GtEq, ParseBinaryExpr },
            { TokenKind.Lt, ParseBinaryExpr },
            { TokenKind.LtEq, ParseBinaryExpr },
            { TokenKind.EqEq, ParseBinaryExpr },
        };
    }

    internal void Parse()
    {
        while (IsNotEof())
        {
            var stmt = ParseStmt();
            Program.Add(stmt);
        }
    }

    internal Stmt ParseStmt()
    {
        switch (_currToken.Kind)
        {
            case TokenKind.Print:
                return ParsePrintStmt();
            case TokenKind.PrintAst:
                return ParsePrintAstStmt();
            default:
                return ParseExprStmt((int)BindingPower.None);
        }
    }

    internal Stmt ParseExprStmt(int bindingPower)
    {
        Expr expr = ParseExpr(bindingPower);
        Advance();
        Expect(TokenKind.Semicolon, "");
        return new ExprStmt(expr);
    }

    internal Stmt ParsePrintStmt()
    {
        Advance();
        Expr expr = ParseExpr((int)BindingPower.None);
        Advance();
        Expect(TokenKind.Semicolon, "Print statement must end with a semicolon");
        return new PrintStmt(expr);
    }

    internal Stmt ParsePrintAstStmt()
    {
        Advance();
        Expr expr = ParseExpr((int)BindingPower.None);
        Advance();
        Expect(TokenKind.Semicolon, "PrintAst statement must end with a semicolon");
        return new PrintAstStmt(expr);
    }

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
        if (_readPos >= _tokens.Count)
            return _tokens[_tokens.Count - 1];

        return _tokens[_readPos];
    }

    private void Match(TokenKind kind, string msg)
    {
        if (_currToken.Kind != kind)
        {
            if (msg == "" || msg == null)
            {
                msg = String.Format("Error: Expected token of type {0}, found token of type {1}\n", kind.ToString(), _currToken.Kind.ToString());
            }
            Console.WriteLine(msg);
            Environment.Exit(1);
        }
    }

    private Token Expect(TokenKind kind, string msg)
    {
        if (_currToken.Kind != kind)
        {
            if (msg == "" || msg == null)
            {
                msg = String.Format("Error: Expected token of type {0}, found token of type {1}\n", kind.ToString(), _currToken.Kind.ToString());
            }
            Console.WriteLine(msg);
            Environment.Exit(1);
        }
        return Advance();
    }
}
