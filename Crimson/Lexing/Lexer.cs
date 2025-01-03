using System.Text;

namespace Crimson.Lexing;

internal class Lexer
{
    private readonly string _source;
    private int _pos;
    private int _readPos;
    private char _ch;

    private Dictionary<string, TokenKind> _keywords = new Dictionary<string, TokenKind>()
    {
        { "proc", TokenKind.Proc },    // PROC
        { "number", TokenKind.Number },    // NUMBER
        { "let", TokenKind.Let },      // LET
        { "const", TokenKind.Const },  // CONST
        { "if", TokenKind.If },        // IF
        { "else", TokenKind.Else },    // ELSE
        { "return", TokenKind.Return },// RETURN
        { "print", TokenKind.Print },  // PRINT
        { "loop", TokenKind.Loop },    // LOOP
        { "break", TokenKind.Break },   // BREAK
        { "true", TokenKind.True },   // BREAK
        { "false", TokenKind.False },   // BREAK
    };

    internal List<Token> Tokens { get; set; } = new List<Token>();    

    internal Lexer(string source)
    {
        _source = source;
        _pos = 0;
        _readPos = 1;
        _ch = _source[0];
    }

    public List<Token> LexTokens()
    {
        for (Token token = LexToken(); token.Kind != TokenKind.Eof; token = LexToken())
        {
            Tokens.Add(token);
        }
        Tokens.Add(new Token(TokenKind.Eof, "EOF"));

        return Tokens;
    }

    public Token LexToken()
    {
        Token token = new Token(TokenKind.Illegal, "ILLEGAL");

        SkipWhitespace();

        switch (_ch)
        {
            case '+':
                token = new Token(TokenKind.Plus, "+"); break;
            case '-':
                token = new Token(TokenKind.Minus, "-"); break;
            case '*':
                token = new Token(TokenKind.Star, "*"); break;
            case '/':
                token = new Token(TokenKind.Slash, "/"); break;
            case '(':
                token = new Token(TokenKind.LParen, "("); break;
            case ')':
                token = new Token(TokenKind.RParen, ")"); break;
            case '{':
                token = new Token(TokenKind.LBrace, "{"); break;
            case '}':
                token = new Token(TokenKind.RBrace, "}"); break;
            case ',':
                token = new Token(TokenKind.RBrace, ","); break;
            case ';':
                token = new Token(TokenKind.Semicolon, ";"); break;
            case '.':
                token = new Token(TokenKind.Dot, "."); break;
            case '<':
                token = TwoCharToken('=', TokenKind.LtEq, TokenKind.Lt);
                break;
            case '>':
                token = TwoCharToken('=', TokenKind.GtEq, TokenKind.Gt);
                break;
            case '=':
                token = TwoCharToken('=', TokenKind.EqEq, TokenKind.Eq);
                break;
            case '!':
                token = TwoCharToken('=', TokenKind.BangEq, TokenKind.Bang);
                break;
            case '\0':
                token = new Token(TokenKind.Eof, "EOF");
                break;
            default:
                {
                    if (IsLetter(_ch))
                    {
                        string ident = ReadIdent();
                        if (_keywords.TryGetValue(ident, out TokenKind kind))
                        {
                            return new Token(kind, ident);
                        }
                        return new Token(TokenKind.Ident, ident) ;
                    }
                    if (IsDigit(_ch))
                    {
                        string numLit = ReadNum();
                        return new Token(TokenKind.NumLit, numLit);
                    }
                }
                break;
        }

        Advance();
        return token;
    }

    private void SkipWhitespace()
    {
        while (_ch == ' ' || _ch == '\n' || _ch == '\r' || _ch == '\r' || _ch == '\t')
        {
            Advance();
        }
    }

    private Token TwoCharToken(char toMatch, TokenKind success, TokenKind other)
    {
        string literal = Char.ToString(_ch);
        if (Peek() == toMatch)
        {
            Advance();
            literal += _ch;
            return new Token(success, literal);
        }

        return new Token(other, literal);
    }

    private char Peek()
    {
        if (_readPos >= _source.Length) { return _ch = '\0'; }

        return _source[_readPos];
    }

    private char Advance()
    {
        if (_readPos >= _source.Length) { return _ch = '\0'; }

        _pos++;
        _readPos++;
        _ch = _source[_pos];
        return _source[_pos];
    }

    private string ReadIdent()
    {
        StringBuilder literal = new("");
        while (IsAlphaNumeric(_ch))
        {
            literal.Append(_ch);
            Advance();
        }
        
        return literal.ToString();
    }

    private string ReadNum()
    {
        StringBuilder literal = new("");
        while (IsDigit(_ch))
        {
            literal.Append(_ch);
            Advance();
        }

        if (_ch == '.' && IsDigit(Peek()))
        {
            literal.Append(_ch);
            Advance();
            while (IsDigit(_ch))
            {
                literal.Append(_ch);
                Advance();
            }
        }

        return literal.ToString();
    }

    private bool IsDigit(char ch)
    {
        return ch >= '0' && ch <= '9';
    }

    private bool IsLetter(char ch)
    {
        return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
    }

    private bool IsAlphaNumeric(char ch)
    {
        return IsLetter(ch) || IsDigit(ch) || ch == '_';
    }
}
