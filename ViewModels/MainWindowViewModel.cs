using Avalonia.Media; // Required for Colors
using CommunityToolkit.Mvvm.ComponentModel; // Required for ObservableObject and ObservableProperty
using CommunityToolkit.Mvvm.Input;          // Required for RelayCommand
using PixelWallE.Models; // Your models namespace
using System.Diagnostics;
using System.Threading.Tasks;

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
        StatusText = "Applying resize..."; // Update status
        Debug.WriteLine($"Resizing canvas to {CanvasSize}x{CanvasSize}");
        if (CanvasSize < 1) CanvasSize = 1;
        PixelCanvas = new PixelCanvas(CanvasSize, CanvasSize);
        _wallE = new WallE(PixelCanvas);
        UpdateWallEPosition();
        StatusText = $"Canvas resized to {CanvasSize}x{CanvasSize}. Ready."; // Update status
    }

    [RelayCommand]
    private void ExecuteCode()
    {
        StatusText = "Executing code (Placeholder)..."; // Update status
        Debug.WriteLine($"--- Execute Code button pressed ---");
        Debug.WriteLine("ExecuteCode: Running test sequence as placeholder.");

        // --- TEMPORARY: Call the test sequence ---
        RunTestSequence(); // This will update the status again upon completion/error
        // --- END TEMPORARY ---

        // When you implement the real parser, update StatusText based on parsing/runtime errors or success.
        // e.g., StatusText = "Code executed successfully.";
        // e.g., StatusText = $"Error on line {lineNumber}: Invalid command.";
    }
}