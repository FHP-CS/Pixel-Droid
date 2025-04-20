// Common/ParsingError.cs (o Parser/ParsingError.cs)
namespace PixelWallE.Common; // o PixelWallE.Parser

public class ParsingError
{
    public string Message { get; }
    public int Line { get; }
    public int Column { get; }
    public ErrorType Type { get; }

    public ParsingError(string message, int line, int column, ErrorType type = ErrorType.Syntax)
    {
        Message = message;
        Line = line;
        Column = column;
        Type = type;
    }

    public override string ToString()
    {
        return $"[{Type} Error L:{Line} C:{Column}] {Message}";
    }
}

public enum ErrorType
{
    Lexical,
    Syntax,
    Semantic,
    Runtime
}