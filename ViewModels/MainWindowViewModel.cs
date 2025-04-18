// ViewModels/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelWallE.Models; // Your models namespace
using System.Diagnostics;
using Avalonia.Media;
using System.Threading.Tasks; // For async commands if needed

namespace PixelWallE.ViewModels;

public partial class MainWindowViewModel : ObservableObject // Inherit from ObservableObject
{
    // Make PixelCanvas observable so the View updates when it's replaced (e.g., on resize)
    [ObservableProperty]
    private PixelCanvas _pixelCanvas;

    // WallE interacts with the PixelCanvas
    private WallE _wallE;

    // Example property for code text editor (implement later)
    [ObservableProperty]
    private string _codeText = ""; // Initialize with some default/test code

    // Example property for canvas size input (implement later)
    [ObservableProperty]
    private int _canvasSize = 32; // Default size

    public MainWindowViewModel()
    {
        // Initialize canvas and WallE
        _pixelCanvas = new PixelCanvas(_canvasSize, _canvasSize);
        _wallE = new WallE(_pixelCanvas);

        // Set some default code for testing Spawn and DrawLine
        CodeText = """
                   Spawn(5, 5)
                   Color(Black)
                   Size(1)
                   DrawLine(1, 0, 10) // Right
                   Color(Blue)
                   Size(3)
                   DrawLine(0, 1, 10) // Down
                   Color(Red)
                   Size(5)
                   DrawLine(-1, 1, 8) // Up-Left Diagonal
                   """;
    }

    // Example Command to run the *hardcoded* test sequence
    [RelayCommand]
    private void RunTestSequence()
    {
        Debug.WriteLine("--- Running Test Sequence ---");
        // Reset canvas to white before running the test sequence
        PixelCanvas.Clear(Colors.White);
        Debug.WriteLine("Canvas cleared for test sequence.");
        // **** END OF ADDITION ****

        // Reset Wall-E's internal state as well if needed, although Spawn does this implicitly
        // If Spawn fails, the rest shouldn't run anyway
        if (_wallE.Spawn(5, 5)) // Check return value
        {
            _wallE.SetColor("Black");
            _wallE.SetSize(1);
            _wallE.DrawLine(1, 0, 10); // Right

            _wallE.SetColor("Blue");
            _wallE.SetSize(3);
            _wallE.DrawLine(0, 1, 10); // Down

            _wallE.SetColor("Green");
            _wallE.SetSize(5);
            _wallE.DrawLine(-1, 1, 8); // Up-Left Diagonal
        }
        else
        {
             Debug.WriteLine("Execution failed: Could not spawn Wall-E.");
             // Maybe show an error message to the user
        }
         Debug.WriteLine("--- Test Sequence Finished ---");

        // Note: The PixelCanvasControl should update automatically
        // because WallE calls _pixelCanvas.NotifyChanged() -> OnCanvasInvalidated -> InvalidateVisual()
    }

    // Example Command for resizing (implement later)
    [RelayCommand]
    private void ApplyResize()
    {
         Debug.WriteLine($"Resizing canvas to {CanvasSize}x{CanvasSize}");
         // Recreate or resize the existing canvas
         // Recreating is simpler for now and matches spec (clears canvas)
         PixelCanvas = new PixelCanvas(CanvasSize, CanvasSize); // Assign to the ObservableProperty
         _wallE = new WallE(PixelCanvas); // WallE needs the new canvas instance
         // Note: Recreating PixelCanvas automatically triggers update in the View
         // because PixelCanvas is an ObservableProperty.
    }

    // Add Commands for Load, Save, Run Code (parse _codeText) later
    [RelayCommand]
    private void ExecuteCode()
    {
        // TODO: Implement the parser and interpreter for CodeText
        Debug.WriteLine("ExecuteCode: Not implemented yet.");
        // For now, maybe just run the test sequence?
         RunTestSequence();
    }
}