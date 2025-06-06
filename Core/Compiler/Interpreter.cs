using PixelWallE.Common; // For Token, TokenType
using PixelWallE.Parser; // For AstNode and its derivatives
using PixelWallE.Models; // For PixelCanvas, WallE
using Avalonia.Media; // For Colors
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; // For Debug.WriteLine
namespace PixelWallE.Execution
{
    public class Interpreter
    {
        // --- Core Models ---
        public PixelCanvas Canvas { get; private set; }
        public WallE WallEInstance { get; set; }

        // --- Program State ---
        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();
        private readonly Dictionary<string, int> _labelTable = new Dictionary<string, int>(); // Label name -> statement index
        private int _programCounter = 0;
        private List<StatementNode> _statements;
        private bool _isSpawned = false;// Track if Spawn command has been executed
        //COnstructor
        public Interpreter(PixelCanvas canvas, WallE walle)
        {
            Canvas = canvas;
            WallEInstance = walle;
        }
        //Methods
        public void Reset()
        {
            Canvas.Clear(Colors.White);
            WallEInstance = new WallE(Canvas);
            _isSpawned = false;
        }
        public ParsingError? Run(ProgramNode program)
        {
            if (_statements != null)
                Reset();

            _statements = program.Statements;
            _programCounter = 0;
            _variables.Clear();
            _labelTable.Clear();
            _isSpawned = false;
            //Fill labeltable
            for (int i = 0; i < _statements.Count; i++)
            {
                if (_statements[i] is LabelNode labelNode)
                {
                    if (_labelTable.ContainsKey(labelNode.Name))
                    {
                        ReportError($"Duplicate label definition: '{labelNode.Name}'", labelNode.LabelToken);
                        throw new RuntimeError($"Duplicate label definition: '{labelNode.Name}'", labelNode.LabelToken);
                    }
                    _labelTable[labelNode.Name] = i;
                }
            }

            //check spawn first non label 
            int firstIndexExecutable = -1;
            for (int i = 0; i < _statements.Count; i++)
            {
                if (!(_statements[i] is LabelNode))
                    firstIndexExecutable = i;
                break;
            }
            if (firstIndexExecutable != -1 && !(_statements[firstIndexExecutable] is SpawnNode))
            {
                // Try to get a token from the first non-Spawn statement for error reporting
                Token errorToken = GetFirstTokenOfStatement(_statements[firstIndexExecutable]);
                ReportError("The first executable command in the script must be 'Spawn'.", errorToken);
                throw new RuntimeError("Cannot execute commands before Wall-E is successfully spawned.", errorToken);
            }
            // Main execution loop
            while (_programCounter < _statements.Count)
            {
                StatementNode currentStatement = _statements[_programCounter];
                _programCounter++;
                if (!_isSpawned && !(currentStatement is SpawnNode) && !(currentStatement is LabelNode))
                {
                    Token errorToken = GetFirstTokenOfStatement(currentStatement);
                    throw new RuntimeError("Wall-E must be spawned using 'Spawn(x,y)' before other commands can be executed.", errorToken);
                }
                try
                {
                    currentStatement.Execute(this);
                    // If SpawnNode was just executed successfully, mark it.
                    if (currentStatement is SpawnNode) _isSpawned = true;
                }
                catch (RuntimeError ex)
                {
                    ReportError(ex.Message, ex.Token);
                    return new ParsingError(ex.Message, ex.Token.Line, ex.Token.Column, ErrorType.Runtime);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UNEXPECTED RUNTIME ERROR: {ex.Message} at statement {_programCounter - 1}");
                    Debug.WriteLine(ex.StackTrace);
                    ReportError($"An unexpected internal error occurred: {ex.Message}", GetFirstTokenOfStatement(currentStatement));
                    return new ParsingError(ex.Message, -1, -1, ErrorType.Runtime);

                }

            }
            return null;
        }
        private Token GetFirstTokenOfStatement(StatementNode statement)
        {
            // This is a bit tricky as AST nodes don't always store their first token.
            // We might need to enhance AST nodes or pass tokens around more.
            // For now, a placeholder.
            if (statement is SpawnNode sn && sn.X is AstNode xn) return null; // Placeholder
            // A better way is to have each statement node store its primary token (e.g., the keyword token)
            // e.g., public class SpawnNode : StatementNode { public Token SpawnKeywordToken; ... }
            // For now, returning null means the error message won't have precise line/col from token.
            return null;
        }


        //--- Public methods for AST nodes to interact with Interpreter's state ---
        // These are now mostly delegates or getters for variables/labels.
        // Wall-E actions are on WallEInstance. Canvas actions are on Canvas.

        public object GetVariable(string name, Token tokenForError)
        {
            if (_variables.TryGetValue(name, out object value))
            {
                return value;
            }
            throw new RuntimeError($"Undefined variable '{name}'.", tokenForError);
        }

        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
        }
        public int GetLabelAddress(string name, Token tokenForError)
        {
            if (_labelTable.TryGetValue(name, out int address))
            {
                return address;
            }
            throw new RuntimeError($"Undefined label '{name}'.", tokenForError);
        }
        public void GoToAddress(int address)
        {
            if (address < 0 || address >= _statements.Count)
            {
                throw new RuntimeError($"GoTo address {address} is out of program bounds.", null);
            }
            _programCounter = address;
        }
        // --- Helper for error reporting ---
        private void ReportError(string message, Token token)
        {
            // Simple console error reporting for now
            // In a UI app, you might raise an event or update a status bar.
            string errorLocation = "";
            if (token != null)
            {
                errorLocation = $"[line {token.Line}, col {token.Column}, token '{token.Lexeme}'] ";
            }
            Debug.WriteLine($"RUNTIME ERROR: {errorLocation}{message}");
            // You might want to throw a specific exception here that the UI can catch
            // or have an event that the UI subscribes to for errors.
            // For a console/backend, re-throwing or just logging might be enough.
        }
        public object CallFunction(string FunctionName, List<object> args, Token FunctionToken)
        {
            string nameLower = FunctionName.ToLowerInvariant();
            switch (nameLower)
            {
                case "getactualx":
                    {
                        EnsureArgumentCount(FunctionName, args, 0, FunctionToken);
                        if (!_isSpawned) throw new RuntimeError("Wall-E must be spawned to get its position.", FunctionToken);
                        return WallEInstance.X;
                    }
                case "getactualy":
                    {
                        EnsureArgumentCount(FunctionName, args, 0, FunctionToken);
                        if (!_isSpawned) throw new RuntimeError("Wall-E must be spawned to get its position.", FunctionToken);
                        return WallEInstance.Y;
                    }
                //added 
                case "getbrushsize":
                    {
                        EnsureArgumentCount(FunctionName, args, 0, FunctionToken);
                        if (!_isSpawned) throw new RuntimeError("Wall-E must be spawned to get its brush size.", FunctionToken);
                        return WallEInstance.BrushSize;
                    }
                case "getcanvassize":
                    {
                        EnsureArgumentCount(FunctionName, args, 0, FunctionToken);
                        return Canvas.Width;
                    }
                case "getcolorcount":
                    {
                        EnsureArgumentCount(FunctionName, args, 5, FunctionToken);
                        string colorname = ExpectString(args[0], FunctionName, "first (color_name)", FunctionToken);
                        Color color = ParseColorName(colorname, FunctionToken, $"Invalid color name {colorname}.");
                        int x1 = ExpectInt(args[1], FunctionName, "second (x1)", FunctionToken);
                        int y1 = ExpectInt(args[2], FunctionName, "second (y1)", FunctionToken);
                        int x2 = ExpectInt(args[3], FunctionName, "second (x2)", FunctionToken);
                        int y2 = ExpectInt(args[4], FunctionName, "second (y2)", FunctionToken);
                        return Canvas.GetColorCount(color, x1,y1,x2,y2);
                    }
                case "isbrushcolor":
                    {
                        EnsureArgumentCount(FunctionName, args, 1, FunctionToken);
                        string colorname = ExpectString(args[0], FunctionName, "first (color_name)", FunctionToken);
                        Color targetcolor = ParseColorName(colorname, FunctionToken, $"Invalid color name {colorname}.");
                        Color actualcolor = WallEInstance.BrushColor;
                        return targetcolor == actualcolor ? 1 : 0;
                    }
                case "isbrushsize":
                    {
                        EnsureArgumentCount(FunctionName, args, 1, FunctionToken);
                        int targetsize = ExpectInt(args[0], FunctionName, "arg size", FunctionToken);
                        int actualsize = WallEInstance.BrushSize;
                        return targetsize == actualsize ? 1 : 0;
                    }
                case "iscanvascolor":
                    {
                        EnsureArgumentCount(FunctionName, args, 3, FunctionToken);
                        string colorname = ExpectString(args[0], FunctionName, "first (color_name)", FunctionToken);
                        Color colorTarget = ParseColorName(colorname, FunctionToken, $"Invalid color name '{colorname}' for IsCanvasColor.");

                        int xpos = ExpectInt(args[1], FunctionName, "second (x)", FunctionToken);
                        int ypos = ExpectInt(args[2], FunctionName, "third (y)", FunctionToken);
                        int xCoord = WallEInstance.X + xpos;
                        int yCoord = WallEInstance.Y + ypos;
                        if (!(xCoord >= 0 && xCoord < Canvas.Width && yCoord >= 0 && yCoord < Canvas.Height)) return false;//out of bounds
                        Color actualcolor = Canvas.GetPixel(xCoord, yCoord);
                        return colorTarget == actualcolor ? 1 : 0;
                    }
            }
            throw new RuntimeError($"Undefined function '{FunctionName}'.", FunctionToken);
        }
        private Color ParseColorName(string colorName, Token tokenForError, string errorMessage)
        {
            return colorName.ToLowerInvariant() switch
            {
                "red" => Colors.Red,
                "blue" => Colors.Blue,
                "green" => Colors.Green,
                "yellow" => Colors.Yellow,
                "orange" => Colors.Orange,
                "purple" => Colors.Purple,
                "black" => Colors.Black,
                "white" => Colors.White,
                "transparent" => Colors.Transparent,
                _ => throw new RuntimeError(errorMessage, tokenForError)
            };
        }
        private string ExpectString(object arg, string funcName, string argDescription, Token funcToken)
        {
            if (arg is string str) return str;
            throw new RuntimeError($"Argument {argDescription} for function '{funcName}' must be a string. Got {arg?.GetType().Name ?? "null"}.", funcToken);
        }
        private int ExpectInt(object arg, string funcName, string argDescription, Token funcToken)
        {
            if (arg is int intArg) return intArg;
            throw new RuntimeError($"Argument {argDescription} for function '{funcName}' must be an integer. Got {arg?.GetType().Name ?? "null"}.", funcToken);
        }
        private void EnsureArgumentCount(string funcName, List<object> args, int expectedCount, Token funcToken)
        {
            if (args.Count != expectedCount)
            {
                throw new RuntimeError($"Function '{funcName}' expected {expectedCount} argument(s) but got {args.Count}.", funcToken);
            }
        }
    }
}