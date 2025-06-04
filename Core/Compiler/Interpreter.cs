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
        public void Run(ProgramNode program)
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
                        return;
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
                return;
            }
            // Main execution loop
            while (_programCounter < _statements.Count)
            {
                StatementNode currentStatement = _statements[_programCounter];
                _programCounter++;
                if (!_isSpawned && !(currentStatement is SpawnNode) && !(currentStatement is LabelNode))
                {
                    ReportError("Wall-E must be spawned using 'Spawn(x,y)' before other commands can be executed.", GetFirstTokenOfStatement(currentStatement));
                    return;
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
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UNEXPECTED RUNTIME ERROR: {ex.Message} at statement {_programCounter-1}");
                    Debug.WriteLine(ex.StackTrace);
                    ReportError($"An unexpected internal error occurred: {ex.Message}", GetFirstTokenOfStatement(currentStatement));
                    return;
                }
                
            }
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
            if(_labelTable.TryGetValue(name, out int address))
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
    }
}