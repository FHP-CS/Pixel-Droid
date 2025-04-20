// Common/Token.cs (o Parser/Token.cs)
namespace PixelWallE.Common; // o PixelWallE.Parser

public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; } // El texto original del token
    public object? Literal { get; } // El valor (e.g., int 10, string "Red")
    public int Line { get; }
    public int Column { get; } // Columna donde empieza el token

    public Token(TokenType type, string lexeme, object? literal, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"{Type} '{Lexeme}' [L:{Line} C:{Column}]" + (Literal != null ? $" ({Literal})" : "");
    }
}