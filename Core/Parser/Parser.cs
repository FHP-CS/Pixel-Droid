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
        // Filter unknown
        _tokens = tokens.Where(t => t.Type != TokenType.Unknown).ToList();
    }

    public (ProgramNode Program, List<ParsingError> Errors) Parse()
    {
        // Reset state for potential re-parsing
        _current = 0;
        _errors.Clear();
        _commands.Clear();
        _spawnEncountered = false;
        ProgramNode program = new ProgramNode();

        while (!IsAtEnd() && Peek().Type != TokenType.EOF) // Continue until EOF
        {
            try
            {
                if (Peek().Type == TokenType.EOF) break; // Reached end after EOLs

                // Skip EOLs at the beginning of parsing or between statements
                while (Peek().Type == TokenType.EOL && !IsAtEnd())
                {
                    Advance();
                }

                StatementNode statement = ParseStatement();

                if (statement != null)
                {
                    program.Statements.Add(statement);
                }
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

        return (program, _errors);
    }

    private StatementNode ParseStatement()
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
            case TokenType.Identifier:
            {
                // Could be an assignment: IDENTIFIER ASSIGN ...
                // Could be a label: IDENTIFIER EOL (if labels are just identifier then newline)
                // We'll need to peek ahead.
                return ParseAssignmentOrLabelStatement(); // A new method to decide
            }
            case TokenType.EOF:
                return null; // End of file, no more statements
            
            default:
                // Unexpected token at the start of a statement
                Error(currentToken, $"Expected a statement but found '{currentToken.Lexeme}");
                Advance(); // Consume the unexpected token to try and proceed
                return null; // Indicate statement parsing failed
        }
    }

    // --- Command Parsing Methods ---

    private StatementNode ParseSpawnCommand()
    {
        Token keywordToken = Advance(); // Consume 'Spawn'
        Consume(TokenType.LParen, "Expected '(' after Spawn.");
        ExpressionNode xToken = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after X coordinate.");
        ExpressionNode yToken = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after Y coordinate.");

        return new SpawnNode(x, y);
    }

    private StatementNode ParseColorCommand()
    {
        Token keywordToken = Advance(); // Consume 'Color'
        Consume(TokenType.LParen, "Expected '(' after Color.");
        // PDF shows Color("Red"), implying a string literal. Lexer handles known colors -> TokenType.String
        ExpressionNode colorToken = Consume(TokenType.String, "Expected color name (e.g., Red, Blue) for Color.");
        Consume(TokenType.RParen, "Expected ')' after color name.");

        string colorName = colorToken.Lexeme; // Use the original lexeme
        return new ColorNode(colorToken);
    }

    private StatementNode ParseSizeCommand()
    {
        Token keywordToken = Advance(); // Consume 'Size'
        Consume(TokenType.LParen, "Expected '(' after Size.");
        ExpressionNode size = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after size.");

        return new SizeNode(size);
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

    //Arithmetic
    private ExpressionNode ParseFactor()
    {
        Token token = Peek();
        if (Peek().Type == TokenType.Number)
        {
            Consume(TokenType.Number, "Expected Number"); // Consume the number token
            return new NumberNode(int.Parse(token.Literal.ToString()));
        }
        else if (token.Type == TokenType.LParen)
        {
            Consume(TokenType.LParen, "Expected Left Pharentesis."); // Consume '('
            ExpressionNode node = ParseExpression(); // Parse the expression inside parentheses
            Consume(TokenType.RParen, "Exected Left Pharentesis"); // Consume ')'
            return node;
        }
        // Handle unary plus/minus (optional, more advanced)
        // else if (token.Type == TokenType.PLUS || token.Type == TokenType.MINUS) { ... }
        else
        {
            Error(token, $"Parser Error: Expected NUMBER or LPAREN but found {token.Type} ('{token.Literal}') at position {_current}");
            return null; // Indicate statement parsing failed
        }
    }
    private ExpressionNode ParsePower()
    {
        ExpressionNode node = ParseFactor();//first factor
        if (Peek().Type == TokenType.Power)
        {
            Token opToken = Peek();
            Consume(TokenType.Power, "Expected Multiplication operator.");
            ExpressionNode right = ParseFactor(); //second factor
            node = new BinaryOpNode(node, opToken, right);
        }
        return node;
    }
    private ExpressionNode ParseTerm()
    {
        ExpressionNode node = ParsePower();//first factor
        while (Peek().Type == TokenType.Multiply || Peek().Type == TokenType.Divide || Peek().Type == TokenType.Modulo)
        {
            Token opToken = Peek();
            if (opToken.Type == TokenType.Multiply)
                Consume(TokenType.Multiply, "Expected Multiplication operator.");
            if (opToken.Type == TokenType.Divide)
                Consume(TokenType.Divide, "Expected Division operator.");
            if (opToken.Type == TokenType.Modulo)
                Consume(TokenType.Modulo, "Expected Modulo operator");
            ExpressionNode right = ParsePower(); //second factor
            node = new BinaryOpNode(node, opToken, right);
        }
        return node;
    }
    private ExpressionNode ParseExpression()
    {
        ExpressionNode node = ParseTerm();
        while (Peek().Type == TokenType.Plus || Peek().Type == TokenType.Minus)
        {
            Token opToken = Peek();
            if (opToken.Type == TokenType.Plus)
                Consume(TokenType.Plus, "Expected Plus operator.");
            if (opToken.Type == TokenType.Minus)
                Consume(TokenType.Minus, "Expected Minus operator.");
            ExpressionNode rightNode = ParseTerm(); //second factor
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;

    }


    // --- Helper Methods ---

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    private Token Consume(TokenType expectedType, string errorMessage)
    {
        if (Check(expectedType)) return Advance();
        // Throw specific exception to be caught by Parse()
        throw new ParserException(errorMessage, Peek());
    }
    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
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

    private Token CurrentTokenOrEOF()
    {
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
            if (Previous().Type == TokenType.EOL) return; // End of line is a good sync point
            // If the previous token was potentially an end-of-statement marker (like EOL if used), maybe stop.
            // Or look for start keywords of the next statement.
            switch (Peek().Type)
                {
                    // Keywords that likely start new statements
                    case TokenType.Spawn:
                    case TokenType.Color:
                    case TokenType.Size:
                    case TokenType.DrawLine:
                    case TokenType.DrawCircle:
                    case TokenType.DrawRectangle:
                    // ... add other statement-starting keywords ...
                    case TokenType.GoTo:
                    case TokenType.Identifier: // Could be an assignment or a label
                        return;
                }
                Advance();
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