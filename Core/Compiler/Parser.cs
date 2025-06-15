// Parser/Parser.cs
using PixelWallE.Common;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;
    private readonly List<ParsingError> _errors = new List<ParsingError>();

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
                string Tokens = "";
                for (int i = 0; i < _tokens.Count; i++)
                {
                    Tokens += _tokens[i].ToString() + ",";
                }
                _errors.Add(new ParsingError($"TOkens: {Tokens}\nUnexpected parsing error: {ex.Message}", CurrentTokenOrEOF().Line, CurrentTokenOrEOF().Column, ErrorType.Syntax));
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
                return ParseSpawnStatement();

            case TokenType.Color: return ParseColorStatement();

            case TokenType.Size: return ParseSizeStatement();

            case TokenType.DrawLine: return ParseDrawLineStatement();

            case TokenType.DrawCircle: return ParseDrawCircleStatement();

            case TokenType.DrawRectangle: return ParseDrawRectangleStatement();

            case TokenType.Fill: return ParseFillStatement();

            case TokenType.GoTo: return ParseGoToStatement();

            case TokenType.Identifier:
                {
                    //peek ahead
                    if (PeekNext().Type == TokenType.Assignment)
                    {
                        return ParseAssignmentStatement();
                    }
                    else if (PeekNext().Type == TokenType.EOL || PeekNext().Type == TokenType.EOF)
                    {
                        return ParseLabelStatement();
                    }
                    else
                    {
                        Error(currentToken, $"Identifier not forming valid statement.");
                        Advance(); // Consume the unexpected token to try and proceed
                        return null;
                    }

                }
            case TokenType.EOL:
                {
                    // It's an empty line. Consume EOL and return null (no statement).
                    // The main ParseProgram loop already skips leading EOLs.
                    // This handles EOLs that might be *between* statements or as "empty statements".
                    Advance(); // Consume the EOL
                    return null;
                }
            case TokenType.EOF:
                return null; // End of file, no more statements

            default:
                // Unexpected token at the start of a statement
                // Error(currentToken, $"Expected a statement but found '{currentToken.Lexeme}'.");
                Advance(); // Consume the unexpected token to try and proceed
                return null; // Indicate statement parsing failed
        }
    }

    // --- Statement Parsing Methods ---
    private StatementNode ParseGoToStatement()
    {
        Token GoTo = Consume(TokenType.GoTo, "Expected GoTo declaration.");
        Consume(TokenType.LBracket, "Expected [ after GoTo.");
        Token label = Consume(TokenType.Identifier, "Expected label name inside '[]' for GoTo.");
        Consume(TokenType.RBracket, "Expected ] after Label.");
        Consume(TokenType.LParen, "Expected ( after ].");
        ExpressionNode condition = ParseExpression();
        Consume(TokenType.RParen, "Expected ) after Condition.");

        return new GoToNode(label, condition, GoTo);
    }
    private StatementNode ParseLabelStatement()
    {
        Token labelNameToken = Consume(TokenType.Identifier, "Expected label. ");
        // if (ValidLabel(labelNameToken))
            return new LabelNode(labelNameToken);
        // throw new ParserException("Label must contain at least one symbol of '-' or '_' to be validated. ", labelNameToken);

    }
    private bool ValidLabel(Token labelT)
    {
        for (int i = 0; i < labelT.Lexeme.Length; i++)
        {
            if (labelT.Lexeme[i] == '_' || labelT.Lexeme[i] == '-') return true;
        }
        return false;
    }
    private AssignmentNode ParseAssignmentStatement()
    {
        Token nameToken = Consume(TokenType.Identifier, "Expected a variable name for assignment.");
        Consume(TokenType.Assignment, "Expected '<-' after variable name for assignment.");
        ExpressionNode valueExpression = ParseExpression();
        return new AssignmentNode(nameToken, valueExpression);
    }
    private StatementNode ParseSpawnStatement()
    {
        Token keywordToken = Advance(); // Consume 'Spawn'
        Consume(TokenType.LParen, "Expected '(' after Spawn.");
        ExpressionNode xExpression = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after X coordinate.");
        ExpressionNode yExpression = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after Y coordinate.");

        return new SpawnNode(keywordToken, xExpression, yExpression);
    }

    private StatementNode ParseColorStatement()
    {
        Token keywordToken = Advance(); // Consume 'Color'
        Consume(TokenType.LParen, "Expected '(' after Color.");
        // PDF shows Color("Red"), implying a string literal. Lexer handles known colors -> TokenType.String
        ExpressionNode colorExpression;
        if (MatchColor(Peek()))
        {
            colorExpression = new StringNode(GetColor(Advance()));//Get the string directly
        }
        else { colorExpression = ParseExpression(); } //parse expresion to get color

        Consume(TokenType.RParen, "Expected ')' after color name.");

        return new ColorNode(keywordToken, colorExpression);
    }

    private StatementNode ParseSizeStatement()
    {
        Token keywordToken = Advance(); // Consume 'Size'
        Consume(TokenType.LParen, "Expected '(' after Size.");
        ExpressionNode size = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after size.");

        return new SizeNode(keywordToken, size);
    }

    private StatementNode ParseDrawLineStatement()
    {
        Token keywordToken = Advance(); // Consume 'DrawLine'
        Consume(TokenType.LParen, "Expected '(' after DrawLine.");
        ExpressionNode dirXExp = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after X direction.");
        ExpressionNode dirYExp = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after Y direction.");
        ExpressionNode distExp = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after distance.");

        return new DrawLineNode(dirXExp, dirYExp, distExp, keywordToken);
    }
    private StatementNode ParseDrawCircleStatement()
    {
        Token keywordToken = Advance(); // Consume 'DrawCircle'
        Consume(TokenType.LParen, "Expected '(' after DrawCircle.");
        ExpressionNode dirx = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after dirX.");
        ExpressionNode diry = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after dirY.");
        ExpressionNode radius = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after radius.");

        return new DrawCircleNode(dirx,diry,radius, keywordToken);
    }
    private StatementNode ParseDrawRectangleStatement()
    {
        Token keywordToken = Advance(); // Consume 'DrawRectangle
        Consume(TokenType.LParen, "Expected '(' after DrawRectangle.");
        ExpressionNode width = ParseExpression();
        Consume(TokenType.Comma, "Expected ',' after width.");
        ExpressionNode heigth = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after heigth.");
        return new DrawRectangleNode(width, heigth, keywordToken);
    }
    private StatementNode ParseFillStatement()
    {
        Token keywordToken = Advance(); // Consume 'Fill'
        Consume(TokenType.LParen, "Expected '(' after Fill.");
        Consume(TokenType.RParen, "Expected ')' after (.");
        return new FillNode(keywordToken);

    }

    //EXPRESSIONS
    private ExpressionNode ParseExpression()
    {
        return ParseLogicalAnd();
    }
    private ExpressionNode ParseLogicalAnd()
    {
        ExpressionNode node = ParseLogicalOr();// &&
        while (Match(TokenType.AND))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParseLogicalOr();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }
    private ExpressionNode ParseLogicalOr()// ||
    {
        ExpressionNode node = ParseEquality();
        while (Match(TokenType.OR))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParseEquality();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }
    private ExpressionNode ParseEquality()// ||
    {
        ExpressionNode node = ParseComparison();
        while (Match(TokenType.Equal_Equal))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParseComparison();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }
    private ExpressionNode ParseComparison()// ||
    {
        ExpressionNode node = ParseTerm();
        while (Match(TokenType.Greater_Than, TokenType.Greater_Equal, TokenType.Less_Than, TokenType.Less_Equal))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParseTerm();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }

    private ExpressionNode ParseTerm()//+ -
    {
        ExpressionNode node = ParseFactor();
        while (Match(TokenType.Plus, TokenType.Minus))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParseFactor();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }
    private ExpressionNode ParseFactor()
    {
        ExpressionNode node = ParsePower();//first factor
        while (Match(TokenType.Multiply, TokenType.Modulo, TokenType.Divide))
        {
            Token opToken = Previous();
            ExpressionNode rightNode = ParsePower();
            node = new BinaryOpNode(node, opToken, rightNode);
        }
        return node;
    }
    private ExpressionNode ParsePower()
    {
        ExpressionNode node = ParseUnary();//first factor
        if (Match(TokenType.Power))
        {
            Token opToken = Previous();
            ExpressionNode right = ParseUnary(); //second factor
            node = new BinaryOpNode(node, opToken, right);
        }
        return node;
    }
    private ExpressionNode ParseUnary()
    {
        if (Match(TokenType.Minus))
        {
            Token op = Previous();
            return new UnaryOpNode(op, ParseUnary());
        }
        return ParseCallOrPrimary();
    }

    private ExpressionNode ParseCallOrPrimary()
    {
        if (Check(TokenType.Identifier) && PeekNext().Type == TokenType.LParen)
            return ParseFunctionCall();
        if (Match(TokenType.Number))
            return new NumberNode((int)((double)(Previous().Literal)));
        if (Match(TokenType.String)) // For Color("Red")
            return new StringNode((string)(Previous().Literal));
        if (Match(TokenType.Identifier))
        {
            return new VariableNode(Previous().Lexeme, Previous());
        }
        else if (Match(TokenType.LParen))
        {
            ExpressionNode node = ParseExpression(); // Parse the expression inside parentheses
            Consume(TokenType.RParen, "Exected Right Parenthesis."); // Consume ')'
            return node;
        }
        // Handle unary plus/minus (optional, more advanced)
        // else if (token.Type == TokenType.PLUS || token.Type == TokenType.MINUS) { ... }
        else
        {
            Error(Peek(), $"Parser Error: Expected Number , variable, string Parenthesis, or Function call Expression but found {Peek().Type} ('{Peek().Literal}') at position {_current}.");
            return null; // Indicate statement parsing failed
        }
    }
    private FunctionCallNode ParseFunctionCall()
    {
        //current token is identifier and next token is LParen
        Token functionNameToken = Consume(TokenType.Identifier, "Expected function name."); // Consumes IDENTIFIER
        Consume(TokenType.LParen, "Expected '(' after function name.");     // Consumes LPAREN
        List<ExpressionNode> args = new List<ExpressionNode>();
        if (!Check(TokenType.RParen))// !)
        {
            args.Add(ParseExpression());
            while (Match(TokenType.Comma))
            {
                args.Add(ParseExpression());
            }
        }
        Consume(TokenType.RParen, "Expected ')' after function arguments.");
        return new FunctionCallNode(functionNameToken.Lexeme, functionNameToken, args);
    }





    // --- Helper Methods ---
    private bool MatchColor(Token token)
    {
        TokenType type = token.Type;
        if (type == TokenType.Red ||
           type == TokenType.Blue ||
           type == TokenType.Green ||
           type == TokenType.Yellow ||
           type == TokenType.Orange ||
           type == TokenType.Purple ||
           type == TokenType.Black ||
           type == TokenType.White ||
           type == TokenType.Transparent) return true;
        return false;
    }
    private string GetColor(Token t)
    {
        switch (t.Type)
        {
            case TokenType.Red: return "Red";
            case TokenType.Blue: return "Blue";
            case TokenType.Green: return "Green";
            case TokenType.Yellow: return "Yellow";
            case TokenType.Orange: return "Orange";
            case TokenType.Purple: return "Purple";
            case TokenType.Black: return "Black";
            case TokenType.White: return "White";
            case TokenType.Transparent: return "Transparent";
        }
        throw new ParserException("Expected a color typed token", t);

    }
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
    private Token PeekNext()
    {
        if (_current + 1 >= _tokens.Count) return _tokens[_tokens.Count - 1];//return last, EOL
        return _tokens[_current + 1];
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