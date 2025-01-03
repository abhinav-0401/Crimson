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
