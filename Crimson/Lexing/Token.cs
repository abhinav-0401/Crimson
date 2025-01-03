namespace Crimson.Lexing;

internal record Token(TokenKind Kind, string Literal);

internal enum TokenKind
{
    Illegal, // ILLEGAL
    Eof,     // EOF

    // Identifiers
    Ident,   // IDENT
    NumLit,   // NumLit

    // Operators
    Assign,   // ASSIGN
    Plus,     // PLUS
    Minus,    // MINUS
    Star,     // STAR
    Slash,    // SLASH
    Lt,       // LT
    LtEq,
    Gt,       // GT
    GtEq,
    Eq,       // EQ
    EqEq,
    Bang,      // BANG
    BangEq,    // BANG_EQ

    // Delimiters
    Comma,     // COMMA
    Semicolon, // SEMICOLON
    Dot,       // DOT 
    LParen,    // LPAREN
    RParen,    // RPAREN
    LBrace,    // LBRACE
    RBrace,    // RBRACE

    // Keywords
    Proc,   // PROC
    Number,
    Let,    // LET
    Const,  // CONST
    If,     // IF
    Else,   // ELSE
    Return, // RETURN
    Print,  // PRINT
    PrintAst,
    Loop,   // LOOP
    Break,   // BREAK
    True,
    False,
    Nil,
}
