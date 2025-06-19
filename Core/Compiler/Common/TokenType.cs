// Common/TokenType.cs (o Parser/TokenType.cs)
namespace PixelWallE.Common; // o PixelWallE.Parser

public enum TokenType
{
    // Keywords
    Spawn,
    Color,
    Size,
    DrawLine,
    DrawCircle,
    DrawRectangle,
    GoTo,
    Fill,

    // Literals
    Number,        
    String,        
    Identifier,    // Variables and Labels and FunctionCalls

    //COlors

    // Red,
    // Blue,
    // Green,
    // Yellow,
    // Orange,
    // Purple,
    // Black,
    // White,
    // Transparent,
    // new colors
    // LightBlue,
    // DarkBlue,
    // Pink,
    // Brown,
    // Gray,
    // Grey,
    // Violet,

    //Operators

    Plus,          // +
    Minus,         // -
    Multiply,      // *
    Divide,        // /
    Power,         // **
    Modulo,        // %
    AND,           // &&
    OR,            // ||
    Equal_Equal,   // ==
    Greater_Equal, // >=
    Less_Equal,    // <=
    Greater_Than,  // >
    Less_Than,     // <


    // Punctuation
    LParen,        // (
    RParen,        // )
    LBracket,      // [
    RBracket,      // ]
    Comma,         // ,
    Assignment,    // <- 


    // Control
    EOL,           // End Of Line (o NewLine)
    EOF,           // End Of File
    Unknown,       // what are you doing? 
    Error          // Lexical Error Token
}