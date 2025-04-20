// Parser/Parser.cs
using PixelWallE.Common;
using PixelWallE.Runtime.Commands;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;
    private readonly List<ParsingError> _errors = new List<ParsingError>();
    private readonly List<ICommand> _commands = new List<ICommand>();

    // Flag to track if Spawn has been encountered (as per PDF rule)
    private bool _spawnEncountered = false;

    public Parser(List<Token> tokens)
    {
        // Filter out potential EOL tokens if Lexer adds them and Parser doesn't need them
        _tokens = tokens.Where(t => t.Type != TokenType.EOL).ToList();
    }

    public (List<ICommand> Commands, List<ParsingError> Errors) Parse()
    {
        // Reset state for potential re-parsing
        _current = 0;
        _errors.Clear();
        _commands.Clear();
        _spawnEncountered = false;

        // Basic validation: Must start with Spawn (moved to SyntaxValidator)
        // if (Peek().Type != TokenType.Spawn)
        // {
        //     Error(Peek(), "Code must start with a Spawn command.");
        // }

        while (!IsAtEnd())
        {
            try
            {
                ICommand? command = ParseStatement();
                if (command != null)
                {
                    _commands.Add(command);
                }
                // If command is null, an error was likely reported already by ParseStatement
            }
            catch (ParserException ex)
            {
                // Catch specific parser errors and add to list
                _errors.Add(new ParsingError(ex.Message, ex.Token.Line, ex.Token.Column, ErrorType.Syntax));
                Synchronize(); // Attempt to recover and continue parsing
            }
            catch (System.Exception ex) // Catch unexpected errors
            {
                 _errors.Add(new ParsingError($"Unexpected parsing error: {ex.Message}", CurrentTokenOrEOF().Line, CurrentTokenOrEOF().Column, ErrorType.Syntax));
                 Synchronize(); // Attempt to recover
            }
        }

        // Add any remaining errors gathered during parsing
        // _errors.AddRange(parsingErrors); // Add errors list if ParseStatement returns them

        return (_commands, _errors);
    }

    private ICommand? ParseStatement()
    {
        Token currentToken = Peek();
        switch (currentToken.Type)
        {
            case TokenType.Spawn:
                // PDF Rule: Spawn can only be used once (checked later by validator)
                // if (_spawnEncountered) {
                //     Error(currentToken, "Spawn command can only be used once at the beginning.");
                //     // Skip this token? Or parse and let validator handle? Let validator handle.
                // }
                _spawnEncountered = true; // Mark that Spawn was seen
                return ParseSpawnCommand();

            case TokenType.Color:
                return ParseColorCommand();

            case TokenType.Size:
                return ParseSizeCommand();

            case TokenType.DrawLine:
                return ParseDrawLineCommand();

            // TODO: Add cases for DrawCircle, Rect, Fill, Assignment, GoTo, Labels...

            case TokenType.EOF:
                return null; // End of file, no more statements

            default:
                // Unexpected token at the start of a statement
                Error(currentToken, $"Unexpected token '{currentToken.Lexeme}'. Expected a command (Spawn, Color, etc.).");
                Advance(); // Consume the unexpected token to try and proceed
                return null; // Indicate statement parsing failed
        }
    }

    // --- Command Parsing Methods ---

    private ICommand ParseSpawnCommand()
    {
        Token keywordToken = Advance(); // Consume 'Spawn'
        Consume(TokenType.LParen, "Expected '(' after Spawn.");
        Token xToken = Consume(TokenType.Number, "Expected X coordinate (number) for Spawn.");
        Consume(TokenType.Comma, "Expected ',' after X coordinate.");
        Token yToken = Consume(TokenType.Number, "Expected Y coordinate (number) for Spawn.");
        Consume(TokenType.RParen, "Expected ')' after Y coordinate.");

        int x = (int)(xToken.Literal ?? 0);
        int y = (int)(yToken.Literal ?? 0);
        return new SpawnCommand(x, y, keywordToken);
    }

    private ICommand ParseColorCommand()
    {
        Token keywordToken = Advance(); // Consume 'Color'
        Consume(TokenType.LParen, "Expected '(' after Color.");
        // PDF shows Color("Red"), implying a string literal. Lexer handles known colors -> TokenType.String
        Token colorToken = Consume(TokenType.String, "Expected color name (e.g., Red, Blue) for Color.");
        Consume(TokenType.RParen, "Expected ')' after color name.");

        string colorName = colorToken.Lexeme; // Use the original lexeme
        return new ColorCommand(colorName, keywordToken);
    }

     private ICommand ParseSizeCommand()
    {
        Token keywordToken = Advance(); // Consume 'Size'
        Consume(TokenType.LParen, "Expected '(' after Size.");
        Token sizeToken = Consume(TokenType.Number, "Expected size (number) for Size.");
        Consume(TokenType.RParen, "Expected ')' after size.");

        int size = (int)(sizeToken.Literal ?? 1); // Default to 1? Or error if null? Let's assume literal is valid.
        return new SizeCommand(size, keywordToken);
    }

    private ICommand ParseDrawLineCommand()
    {
        Token keywordToken = Advance(); // Consume 'DrawLine'
        Consume(TokenType.LParen, "Expected '(' after DrawLine.");
        Token dirXToken = Consume(TokenType.Number, "Expected X direction (-1, 0, or 1) for DrawLine.");
        Consume(TokenType.Comma, "Expected ',' after X direction.");
        Token dirYToken = Consume(TokenType.Number, "Expected Y direction (-1, 0, or 1) for DrawLine.");
         Consume(TokenType.Comma, "Expected ',' after Y direction.");
         Token distToken = Consume(TokenType.Number, "Expected distance (number) for DrawLine.");
        Consume(TokenType.RParen, "Expected ')' after distance.");

        int dirX = (int)(dirXToken.Literal ?? 0);
        int dirY = (int)(dirYToken.Literal ?? 0);
        int distance = (int)(distToken.Literal ?? 0);

         // Basic validation for direction values (more robust validation in Validator/Runtime)
        if (dirX < -1 || dirX > 1) Error(dirXToken, "X direction must be -1, 0, or 1.");
        if (dirY < -1 || dirY > 1) Error(dirYToken, "Y direction must be -1, 0, or 1.");
        if (dirX == 0 && dirY == 0) Error(dirXToken, "Direction (0, 0) is invalid for DrawLine.");
        if (distance < 0) Error(distToken, "Distance must be non-negative.");


        return new DrawLineCommand(dirX, dirY, distance, keywordToken);
    }


    // --- Helper Methods ---

    private Token Consume(TokenType expectedType, string errorMessage)
    {
        Token current = Peek();
        if (current.Type == expectedType)
        {
            return Advance();
        }
        // Throw specific exception to be caught by Parse()
        throw new ParserException(errorMessage, current);
    }

     private void Error(Token token, string message)
    {
        // Avoid duplicate errors for the same token if Synchronize is called
        if (!_errors.Any(e => e.Line == token.Line && e.Column == token.Column))
        {
             _errors.Add(new ParsingError(message, token.Line, token.Column, ErrorType.Syntax));
        }
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

     private Token Previous()
    {
        return _tokens[_current - 1];
    }

     private Token CurrentTokenOrEOF() {
         if (_current < _tokens.Count)
            return _tokens[_current];
         return _tokens.LastOrDefault() ?? new Token(TokenType.EOF, "", null, -1, -1); // Fallback EOF
     }


    // Basic error recovery: advance until we find a likely start of a new statement (or EOF)
    // This is very basic and might skip valid code after an error.
    private void Synchronize()
    {
        Advance(); // Consume the token that caused the error

        while (!IsAtEnd())
        {
            // If the previous token was potentially an end-of-statement marker (like EOL if used), maybe stop.
            // Or look for start keywords of the next statement.
            switch (Peek().Type)
            {
                case TokenType.Spawn:
                case TokenType.Color:
                case TokenType.Size:
                case TokenType.DrawLine:
                // TODO: Add other statement starting keywords/tokens
                case TokenType.EOF:
                    return; // Found a likely start of a new statement or EOF
            }

            Advance(); // Keep consuming tokens
        }
    }

     // Custom exception for Parser errors
    private class ParserException : System.Exception
    {
        public Token Token { get; } // Token where the error occurred
        public ParserException(string message, Token token) : base(message)
        {
            Token = token;
        }
    }
}