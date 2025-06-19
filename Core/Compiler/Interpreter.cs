using PixelWallE.Common; // For Token, TokenType
using System.Threading.Tasks; // for delay
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
        // Core Models
        public PixelCanvas Canvas { get; private set; }
        public WallE WallEInstance { get; set; }

        // Program State
        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();
        private readonly Dictionary<string, int> _labelTable = new Dictionary<string, int>(); // Label name -> statement index
        private int _programCounter = 0;
        private List<StatementNode> _statements;
        
        public bool _isSpawned = false;// Track if Spawn command has been executed
        //COnstructor
        public Interpreter(PixelCanvas canvas, WallE walle)
        {
            Canvas = canvas;
            WallEInstance = walle;
            _statements = new List<StatementNode>();
        }
        //Methods
        public void Reset()
        {
            WallEInstance = new WallE(Canvas);
            _isSpawned = false;
        }
        private (bool,LabelNode) FillLabelTable()
        {
            //Fill labeltable
            for (int i = 0; i < _statements.Count; i++)
            {
                if (_statements[i] is LabelNode labelNode)
                {
                    if (_labelTable.ContainsKey(labelNode.Name))
                    {
                        ReportError($"Duplicate label definition: '{labelNode.Name}'", labelNode.Token);
                        return (false,labelNode);
                    }
                    _labelTable[labelNode.Name] = i;
                }
            }
            return (true,null!);
        }
        private int GetFirstIndexExecutable()
        {
            int firstIndexExecutable = -1;
            for (int i = 0; i < _statements.Count; i++)
            {
                if (!(_statements[i] is LabelNode))
                {
                    firstIndexExecutable = i;
                    return i;
                }
            }
            throw new NotImplementedException();
        }
        public async Task<RuntimeError?> Run(ProgramNode program)
        {
            // if (_statements != null)
            //     Reset();

            _statements = program.Statements;
            _programCounter = 0;
            _variables.Clear();
            _labelTable.Clear();
            _isSpawned = false;

            //FillLabels
            (bool success, LabelNode ProblemLabel) = FillLabelTable();
            if(!success)   return new RuntimeError($"Duplicate label definition: '{ProblemLabel.Name}'", ProblemLabel.Token);

            //check first spawn non label 
            int firstIndexExecutable = GetFirstIndexExecutable();

            if (firstIndexExecutable != -1 && !(_statements[firstIndexExecutable] is SpawnNode))
            {
                // Try to get a token from the first non-Spawn statement for error reporting
                Token errorToken = _statements[firstIndexExecutable].Token;
                ReportError("The first executable command in the script must be 'Spawn'.", errorToken);
                return new RuntimeError("Cannot execute commands before Wall-E is successfully spawned.", errorToken);
            }


            // Main execution loop
            while (_programCounter < _statements.Count)
            {
                StatementNode currentStatement = _statements[_programCounter];
                _programCounter++;
                if (!_isSpawned && !(currentStatement is SpawnNode) && !(currentStatement is LabelNode))
                {
                    Token errorToken = currentStatement.Token;
                    return new RuntimeError("Wall-E must be spawned using 'Spawn(x,y)' before other commands can be executed.", errorToken);
                }
                try
                {
                    await currentStatement.Execute(this);
                    // If SpawnNode was just executed successfully, mark it.
                    if (currentStatement is SpawnNode) _isSpawned = true;
                }
                catch (RuntimeError ex)
                {
                    ReportError(ex.Message + $", L:{ex.Token.Line} C:{ex.Token.Column}", ex.Token);
                    return new RuntimeError(ex.Message + $", L:{ex.Token.Line} C:{ex.Token.Column}", ex.Token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UNEXPECTED RUNTIME ERROR: {ex.Message} at statement {_programCounter - 1}");
                    Debug.WriteLine(ex.StackTrace);
                    ReportError($"An unexpected internal error occurred: {ex.Message}", currentStatement.Token);
                    return new RuntimeError(ex.Message, currentStatement.Token);

                }

            }
            return null;
        }


        // Public methods for AST nodes to interact with Interpreter's state

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
                throw new RuntimeError($"GoTo address {address} is out of program bounds.", null!);
            }
            _programCounter = address;
        }
        // Helper for error reporting
        private void ReportError(string message, Token token)
        {
            string errorLocation = "";
            if (token != null)
            {
                errorLocation = $"[line {token.Line}, col {token.Column}, token '{token.Lexeme}'] ";
            }
            Debug.WriteLine($"RUNTIME ERROR: {errorLocation}{message}");
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
                        return Canvas.GetColorCount(color, x1, y1, x2, y2);
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
        //FunctionCallsMethods 
        
        //for functionsCall methods
        private Color ParseColorName(string colorName, Token tokenForError, string errorMessage)
        {
            string colorString = colorName.ToLowerInvariant();
            if (WallEInstance._Colors.TryGetValue(colorString, out Color color))
            {
                Debug.WriteLine($"returning color {color}");
                return color;
            }
            else
                throw new RuntimeError(errorMessage, tokenForError);
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