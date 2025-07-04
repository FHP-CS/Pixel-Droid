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
            { "DrawLine", TokenType.DrawLine },
            { "DrawCircle", TokenType.DrawCircle},
            { "DrawRectangle", TokenType.DrawRectangle},
            { "GoTo", TokenType.GoTo},
            { "Fill", TokenType.Fill},
            //Colors
            { "Red", TokenType.Color},
            { "Blue", TokenType.Color},

            { "Pink", TokenType.Color},
            { "Brown", TokenType.Color},
            { "Green", TokenType.Color},
            { "Yellow", TokenType.Color},
            { "Orange", TokenType.Color},
            { "Purple", TokenType.Color},
            { "Black", TokenType.Color},
            { "White", TokenType.Color},
            { "Transparent", TokenType.Color},
            //added colors
            { "LightBlue", TokenType.Color},
            { "DarkBlue", TokenType.Color},
            { "Violet", TokenType.Color},
            { "Gray", TokenType.Color},
            { "Grey", TokenType.Color},

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
            case '[': AddToken(TokenType.LBracket); break;
            case ']': AddToken(TokenType.RBracket); break;
            case ',': AddToken(TokenType.Comma); break;
            case '+': AddToken(TokenType.Plus); break;
            case '-': AddToken(TokenType.Minus); break;
            case '*':
                if (Peek() == '*')
                {
                    AddToken(TokenType.Power);//**
                    Advance();
                }
                else
                    AddToken(TokenType.Multiply);//*
                break;
            case '/':
                if (Peek() == '/')// is a comment
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance(); // Consume and advance
                    }
                }
                else //is a divition
                    AddToken(TokenType.Divide);
                break;
            case '%': AddToken(TokenType.Modulo); break;
            case '<':
                if (Peek() == '-')
                {
                    AddToken(TokenType.Assignment);// <-
                    Advance();
                }
                else if (Peek() == '=')
                {
                    AddToken(TokenType.Less_Equal);// <=
                    Advance();
                }
                else
                    AddToken(TokenType.Less_Than);// <
                break;
            case '>':
                if (Peek() == '=') // ( >= )
                {
                    AddToken(TokenType.Greater_Equal);
                    Advance();
                }
                else              // ( > )
                    AddToken(TokenType.Greater_Than);
                break;
            case '=':
                if (Peek() == '=')// ( == ) 
                {
                    AddToken(TokenType.Equal_Equal);
                    Advance();
                }
                break;
            case '&':
                if (Peek() == '&')
                {
                    Advance(); // Consume the second '&'
                    AddToken(TokenType.AND); // Logical AND (&&)
                }
                else
                {
                    throw new LexerException("Unexpected character: '&'. Did you mean '&&'?", _line, _column - 1);
                }
                break;
            case '|':
                if (Peek() == '|')
                {
                    Advance(); // Consume the second '|'
                    AddToken(TokenType.OR); // Logical OR (||)
                }
                else
                {
                    throw new LexerException("Unexpected character: '|'. Did you mean '||'?", _line, _column - 1);
                }
                break;
            // Ignore whitespace
            case '"': // String literal
                StringLiteral();
                break;

            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            // New lines
            case '\n':
                AddToken(TokenType.EOL); // Add EOL token
                _line++;
                _column = 1; // Reset column at new line
                break;

            default:
                if (IsDigit(c) && IsAlpha(Peek()))
                    throw new LexerException($"Variables or labels shall not start with a number, if you meant to multiply then use the expression ** after the number: '{c}'", _line, _column - 1);
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c)) // Starting character for identifiers/keywords/colors
                {
                    Identifier();
                }
                else
                {
                    throw new LexerException($"Unexpected character: '{c}'  What are ya doing mate?", _line, _column - 1);
                    // AddToken(TokenType.Unknown, c.ToString()); 
                }
                break;
        }
    }

    private void Identifier()
    {
        // Consume alphanumeric characters (and potentially '_' for variables/labels)
        while (IsAlphaNumeric(Peek())) Advance();

        string text = _source.Substring(_start, _current - _start);

        // Check if it's a keyword
        if (_keywords.TryGetValue(text, out TokenType type))
        {
            if (type == TokenType.Color) AddToken(type, text);
            else
            {
                AddToken(type);
            }
        }
        else

        {
            // (for variables/labels later)
            if (text[0] == '-') throw new LexerException($"Variable or Label shall not start with the symbol (-)! What are ya doing mate?", _line, _column - 1);
            if (text[0] == '_') throw new LexerException($"Variable or Label shall not start with the symbol (_)! What are ya doing mate?", _line, _column - 1);
            AddToken(TokenType.Identifier);
        }

    }
    private void StringLiteral()
    {
        // _start is at the opening '"'. Current char c (from ScanToken) was '"'.
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') // String literals usually cannot span lines without escape sequences
            {
                AddToken(TokenType.EOL);
                _line++; _column = 0; // If allowing, update line/col
            }
            Advance();
        }

        if (IsAtEnd()) // Unterminated string
        {
            throw new LexerException("Unterminated string literal.", _line, _start + 1); // _start + 1 is the column of opening quote
        }

        Advance();

        // Extract the string value, excluding the quotes
        string value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, value);
    }

    private bool IsKnownColor(string text)
    {
        var knownColors = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
            { "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent" };
        return knownColors.Contains(text);
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

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
    private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == 'Ñ' || c == 'ñ';
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