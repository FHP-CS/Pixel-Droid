// Common/TokenType.cs (o Parser/TokenType.cs)
namespace PixelWallE.Common; // o PixelWallE.Parser

public enum TokenType
{
    // Keywords
    Spawn,
    Color,
    Size,
    DrawLine,
    // TODO: Add keywords for DrawCircle, DrawRectangle, Fill, var assignment ('<-'), functions, GoTo, labels...

    // Literals
    Number,        // e.g., 10, 5, -1
    String,        // e.g., "Red", "Blue" (aunque en el PDF no usan comillas, lo trataremos como un tipo específico)
    Identifier,    // e.g., variable names, labels

    // Punctuation
    LParen,        // (
    RParen,        // )
    Comma,         // ,
    // TODO: Add operators: Arrow (<-), Plus, Minus, Multiply, Divide, Power, Modulo, Comparators (==, >=, etc.), Logical (&&, ||)

    // Control
    EOL,           // End Of Line (o NewLine)
    EOF,           // End Of File
    Unknown,       // Caracteres no reconocidos
    Error          // Token que representa un error léxico
}