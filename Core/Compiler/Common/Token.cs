// Common/Token.cs (o Parser/Token.cs)
using System;
using Avalonia.Collections;
namespace PixelWallE.Common; // o PixelWallE.Parser

public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; } // El texto original del token
    public object? Literal { get; } // El valor (e.g., int 10, string "Red")
    public double NumericValue { get; private set; }
    public int Line { get; }
    public int Column { get; } // Columna donde empieza el token

    public Token(TokenType type, string lexeme, object? literal, int line, int column)
    {
        Type = type;
        if (type == TokenType.Number)
        {
            Lexeme = lexeme;
            if (double.TryParse(lexeme, out double numValue))
            {
                Literal = numValue;
            }
            else
            {
                Console.WriteLine($"Warning> Could not parse '{lexeme}' as a number for NUMBER Token.");
                Type = TokenType.Unknown;
                //or throw exeption
            }
            Line = line;
            Column = column;
        }
        else
        {
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Column = column;
        }
    }
    public Token(TokenType type, int Line, int Column) : this(type, string.Empty, string.Empty, Line, Column)
    {
        if (type == TokenType.Plus)
            Literal = "+";
        else if (type == TokenType.Minus)
            Literal = "-";
        else if (type == TokenType.Multiply)
            Literal = "*";
        else if (type == TokenType.Divide)
            Literal = "/";
        else if (type == TokenType.LParen)
            Literal = "(";
        else if (type == TokenType.RParen)
            Literal = ")";
    }

    public override string ToString()
    {
        return $"{Type} '{Lexeme}' [L:{Line} C:{Column}]" + (Literal != null ? $" ({Literal})" : "");
    }
}