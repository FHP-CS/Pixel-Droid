using System.Collections.Generic;
using System.Linq;
using Avalonia.Media; // Required for Colors
using CommunityToolkit.Mvvm.ComponentModel; // Required for ObservableObject and ObservableProperty
using CommunityToolkit.Mvvm.Input;          // Required for RelayCommand
using PixelWallE.Models; // Your models namespace
using System.Diagnostics;
using System.Threading.Tasks;

using PixelWallE.Parser;
using PixelWallE.Runtime;
using PixelWallE.Runtime.Commands;
using PixelWallE.Common;
using System.Text;

namespace PixelWallE.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    // --- Observable Properties ---

    [ObservableProperty]
    private PixelCanvas _pixelCanvas;

    private WallE _wallE; // Logic model, not directly bound typically

    [ObservableProperty]
    private string _codeText = "";

    [ObservableProperty]
    private int _canvasSize = 32;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWallESpawned))]
    private int _wallEX = -1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWallESpawned))]
    private int _wallEY = -1;

    // **** ADD THIS PROPERTY ****
    [ObservableProperty]
    private string _statusText = "Ready"; // Initialize with default status
    // **** END OF ADDITION ****

    public bool IsWallESpawned => WallEX >= 0 && WallEY >= 0;

    // --- Constructor ---
    public MainWindowViewModel()
    {
        _pixelCanvas = new PixelCanvas(_canvasSize, _canvasSize);
        _wallE = new WallE(_pixelCanvas);
        CodeText = """
                   Spawn(5, 5)
                   Color(Black)
                   Size(1)
                   DrawLine(1, 0, 10)
                   Color(Blue)
                   Size(3)
                   DrawLine(0, 1, 10)
                   Color(Red)
                   Size(5)
                   DrawLine(-1, -1, 8)
                   """;
        UpdateWallEPosition();
        StatusText = "Application loaded. Ready."; // Example initial status update
    }

    // --- Helper Method ---
    private void UpdateWallEPosition()
    {
        WallEX = _wallE.X;
        WallEY = _wallE.Y;
        // Debug message is fine here
        // Debug.WriteLine($"ViewModel Wall-E position updated: ({WallEX}, {WallEY})");
    }

    // --- Commands ---
    [RelayCommand]
    private void RunTestSequence()
    {
        StatusText = "Running test sequence..."; // Update status
        Debug.WriteLine("--- Running Test Sequence ---");
        PixelCanvas.Clear(Colors.White);
        Debug.WriteLine("Canvas cleared for test sequence.");
        _wallE = new WallE(PixelCanvas);
        UpdateWallEPosition();

        if (_wallE.Spawn(5, 5))
        {
            UpdateWallEPosition();
            _wallE.SetColor("Black");
            _wallE.SetSize(1);
            if (_wallE.DrawLine(1, 0, 10)) UpdateWallEPosition();
            _wallE.SetColor("Blue");
            _wallE.SetSize(3);
            if (_wallE.DrawLine(0, 1, 10)) UpdateWallEPosition();
            _wallE.SetColor("Red");
            _wallE.SetSize(5);
            if (_wallE.DrawLine(-1, -1, 8)) UpdateWallEPosition();
            StatusText = "Test sequence finished successfully."; // Update status
        }
        else
        {
            Debug.WriteLine("Execution failed: Could not spawn Wall-E.");
            StatusText = "Error: Could not spawn Wall-E during test."; // Update status on error
            UpdateWallEPosition();
        }
        Debug.WriteLine("--- Test Sequence Finished ---");
    }

    [RelayCommand]
    private void ApplyResize()
    {
        // ... (existing resize logic) ...
        PixelCanvas = new PixelCanvas(CanvasSize, CanvasSize);
        // CRUCIAL: Create a NEW WallE instance tied to the NEW canvas
        _wallE = new WallE(PixelCanvas);
        UpdateWallEPosition();
        StatusText = $"Canvas resized to {CanvasSize}x{CanvasSize}. Wall-E reset. Ready.";
    }
    // Helper method to update status and debug output with errors
    private void ReportErrors(string phase, List<ParsingError> errors)
    {
        var errorReport = new StringBuilder();
        errorReport.AppendLine($"{phase}:");
        foreach (var error in errors)
        {
            errorReport.AppendLine($"- {error}");
            Debug.WriteLine(error.ToString());
        }
        StatusText = errorReport.ToString().Trim(); // Show errors in status bar
    }

    [RelayCommand]
    private void ExecuteCode()
    {
        StatusText = "Compiling...";
        Debug.WriteLine("--- Starting Code Execution ---");
        Debug.WriteLine("Code:\n" + CodeText);

        // 1. Lexing
        var lexer = new Lexer(CodeText);
        (List<Token> tokens, List<ParsingError> lexerErrors) = lexer.Tokenize();

        if (lexerErrors.Any())
        {
            ReportErrors("Lexer Error(s)", lexerErrors);
            return; // Stop if lexing failed
        }

        Debug.WriteLine("--- Lexing Successful ---");
        // foreach (var token in tokens) Debug.WriteLine(token); // Optional: Print tokens

        // 2. Parsing
        var parser = new Parser.Parser(tokens); // Fully qualify if namespace conflict
        (List<ICommand> commands, List<ParsingError> parserErrors) = parser.Parse();

        if (parserErrors.Any())
        {
            ReportErrors("Parser Error(s)", parserErrors);
            return; // Stop if parsing failed
        }

        if (!commands.Any() && tokens.Any(t => t.Type != TokenType.EOF))
        {
            // This case might indicate the parser failed silently or recovered poorly.
            // If there were tokens but no commands parsed, likely a syntax error wasn't caught/reported well.
            // Or it could be valid (e.g. only comments/whitespace) if those were handled.
            if (!parserErrors.Any()) // If no specific errors were reported, add a generic one
            {
                StatusText = "Parsing completed, but no valid commands found.";
                Debug.WriteLine("Parsing completed, but no valid commands found.");
                // Maybe return here depending on desired behavior for empty/comment-only code
            }
            // If there were errors, ReportErrors was already called.
            return;
        }


        Debug.WriteLine("--- Parsing Successful ---");
        // foreach (var cmd in commands) Debug.WriteLine($"Parsed: {cmd.GetType().Name}"); // Optional: Print commands

        // 3. Syntax Validation
        var validator = new SyntaxValidator(commands);
        List<ParsingError> validationErrors = validator.Validate();
       
        if (validationErrors.Any())
        {
            ReportErrors("Validation Error(s)", validationErrors);
            return; // Stop if validation failed
        }

        Debug.WriteLine("--- Validation Successful ---");

        // 4. Interpretation (Execution)
        StatusText = "Executing...";
        var interpreter = new Interpreter();

        // IMPORTANT: Use the *existing* _wallE and _pixelCanvas instances.
        // Do NOT create new ones here unless specifically intended (like after resize).
        // The PDF states execution continues on the modified canvas.
        ParsingError? runtimeError = interpreter.Run(commands, _wallE, PixelCanvas);

        if (runtimeError != null)
        {
            ReportErrors("Runtime Error", new List<ParsingError> { runtimeError });
            // Status text is already set by ReportErrors
        }
        else
        {
            StatusText = "Execution finished successfully.";
            // Make sure the canvas updates visually after execution
            PixelCanvas.NotifyChanged(); // Force UI update
            UpdateWallEPosition();      // Update Wall-E pos in VM if displayed
        }
        Debug.WriteLine("--- Code Execution Finished ---");
    }
}