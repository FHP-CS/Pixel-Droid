// Parser/Lexer.cs
using PixelWallE.Common; // o el namespace que uses
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE.Parser;

public class Lexer
{
    private readonly string _source;
    private readonly List<Token> _tokens = new List<Token>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private int _column = 1; // Track column for better error reporting

    private readonly Dictionary<string, TokenType> _keywords;

    public Lexer(string source)
    {
        _source = source;
        _keywords = new Dictionary<string, TokenType>(System.StringComparer.OrdinalIgnoreCase) // Case-insensitive
        {
            { "Spawn", TokenType.Spawn },
            { "Color", TokenType.Color },
            { "Size", TokenType.Size },
            { "DrawLine", TokenType.DrawLine }
            // TODO: Add more keywords
        };
    }

    public (List<Token> Tokens, List<ParsingError> Errors) Tokenize()
    {
        List<ParsingError> errors = new List<ParsingError>();

        while (!IsAtEnd())
        {
            _start = _current;
            try
            {
                ScanToken();
            }
            catch (LexerException ex)
            {
                errors.Add(new ParsingError(ex.Message, ex.Line, ex.Column, ErrorType.Lexical));
                // Add an error token or skip? Let's just report and continue for now.
                // We might need a way to synchronize after an error.
                Advance(); // Move past the problematic character
            }
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line, _column));
        return (_tokens, errors);
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            // Single character tokens
            case '(': AddToken(TokenType.LParen); break;
            case ')': AddToken(TokenType.RParen); break;
            case ',': AddToken(TokenType.Comma); break;
            // TODO: Add operators like +, *, /, %, ==, >=, <=, >, <, &, |

            // Minus sign needs lookahead (could be negative number)
            case '-':
                if (IsDigit(Peek()))
                {
                    Number(); // Handle negative number
                }
                else
                {
                     throw new LexerException($"Unexpected character '{c}'. Did you mean a negative number?", _line, _column -1);
                    // TODO: Handle minus operator when implemented: AddToken(TokenType.Minus);
                }
                break;

            // Ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            // New lines
            case '\n':
                // AddToken(TokenType.EOL); // Add EOL token
                _line++;
                _column = 1; // Reset column at new line
                break;

            // TODO: Add comments (e.g., // or #)

            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c)) // Starting character for identifiers/keywords/colors
                {
                    IdentifierOrColor();
                }
                else
                {
                    // Report error but don't add error token for now
                     throw new LexerException($"Unexpected character: '{c}'", _line, _column -1);
                    // AddToken(TokenType.Unknown, c.ToString());
                }
                break;
        }
    }

    private void IdentifierOrColor()
    {
        // Consume alphanumeric characters (and potentially '-' as per PDF for variables/labels)
        while (IsAlphaNumeric(Peek()) || Peek() == '-') Advance();

        string text = _source.Substring(_start, _current - _start);

        // Check if it's a keyword
        if (_keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
        }
        else
        {
            // It's not a keyword. Could be a color name or an identifier.
            // For now, treat all non-keyword identifiers as potential color names *or* future variable/label names.
            // The Parser will decide based on context.
            // If it's a valid color name (like "Red", "Blue"), we can treat it as a string literal implicitly.
            // Or, keep it generic Identifier and let parser check if context requires color.
            // Let's treat known colors as string literals implicitly for simplicity now.
            if (IsKnownColor(text))
            {
                 AddToken(TokenType.String, text); // Add as string literal
            }
            else
            {
                 // If not a known color, treat as identifier (for variables/labels later)
                 AddToken(TokenType.Identifier);
                 // TODO: When variables are added, this is correct.
                 // If only known colors are allowed where strings are expected now,
                 // this could be an error if encountered in `Color()` args.
                 // The parser will handle this context check.
            }
        }
    }

    // Basic check for initial color names from PDF
    private bool IsKnownColor(string text)
    {
        var knownColors = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
            { "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent" };
        return knownColors.Contains(text);
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part. (Not specified in PDF, but good practice?)
        // if (Peek() == '.' && char.IsDigit(PeekNext())) { ... }

        string numberString = _source.Substring(_start, _current - _start);
        if (int.TryParse(numberString, out int value))
        {
            AddToken(TokenType.Number, value);
        }
        else
        {
             throw new LexerException($"Invalid number format: '{numberString}'", _line, _start);
        }
    }

    private char Advance()
    {
        _column++;
        return _source[_current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line, _start + 1)); // Column is where token starts
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current + 1];
    }

    private bool IsDigit(char c) => c >= '0' && c <= '9';
    private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'; // Allow underscore? PDF uses '-'
    private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    // Custom exception for Lexer errors
    private class LexerException : System.Exception
    {
        public int Line { get; }
        public int Column { get; }
        public LexerException(string message, int line, int column) : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}