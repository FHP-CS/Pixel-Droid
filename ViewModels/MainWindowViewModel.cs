using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage; // For the new FilePicker API
using System.IO; // For reading and writing files (StreamReader, StreamWriter)
using System.Threading.Tasks;
using Avalonia.Media; // Required for Colors
using CommunityToolkit.Mvvm.ComponentModel; // Required for ObservableObject and ObservableProperty
using CommunityToolkit.Mvvm.Input;          // Required for RelayCommand
using PixelWallE.Models; // Your models namespace
using System.Diagnostics;
using PixelWallE.Parser;
using PixelWallE.Execution;
using System.Windows.Input;       // For ICommand (though IAsyncRelayCommand is more specific)
using PixelWallE.Common;
using Avalonia;
using System.Text;
using System;

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

    //STATUS BAR
    [ObservableProperty]
    public string _statusText = "Ready"; // Initialize with default status
    [ObservableProperty]
    private IBrush _statusBarBrush = Brushes.Black;
    //TYPES
    public enum StatusMessageType
    {
        Info,
        Success,
        Warning,
        Error
    }


    public bool IsWallESpawned => WallEX >= 0 && WallEY >= 0;
    public IAsyncRelayCommand SaveCodeAsyncCommand { get; private set; }
    public IAsyncRelayCommand LoadCodeAsyncCommand { get; private set; } // Load
    public event Action<string>? CodeHasBeenLoaded; // Notify that code


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
                   Color("Blue")
                   Size(3)
                   DrawLine(0, 1, 10)
                   Color(Red)
                   Size(5)
                   DrawLine(-1, -1, 8)
                   """;
        UpdateWallEPosition();
        SetStatus("Application loaded. Ready.", StatusMessageType.Info); // Initialize with a status
        SaveCodeAsyncCommand = new AsyncRelayCommand(SaveCodeAsync_Logic);
        LoadCodeAsyncCommand = new AsyncRelayCommand(LoadCodeAsync_Logic);
    }

    // --- Helper Method ---
    private void SetStatus(string message, StatusMessageType type = StatusMessageType.Info)
    {
        StatusText = message;
        switch (type)
        {
            case StatusMessageType.Success: StatusBarBrush = Brushes.LightGreen; break;
            case StatusMessageType.Error: StatusBarBrush = Brushes.Red; break;
            case StatusMessageType.Warning: StatusBarBrush = Brushes.Orange; break;
            case StatusMessageType.Info:
            default: StatusBarBrush = Brushes.LightGray; break;

        }
    }
    private Window? GetTopLevelWindow()
    {
        // This tries to get the main window of a desktop application.
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
            return desktopApp.MainWindow;
        }
        var allWindows = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows;
        return allWindows?.FirstOrDefault(w => w.IsActive) ?? allWindows?.FirstOrDefault();
    }

    // Event to notify the View that code has been loaded from a file
    private void UpdateWallEPosition()
    {
        WallEX = _wallE.X;
        WallEY = _wallE.Y;
        Debug.WriteLine($"ViewModel Wall-E position updated: ({WallEX}, {WallEY})");
    }

    // --- Commands ---
    // [RelayCommand]
    private async Task SaveCodeAsync_Logic()
    {
        SetStatus("Attempting to save code...", StatusMessageType.Info); // Inform the user

        // 1. Get the parent window for the dialog.
        Window? parentWindow = GetTopLevelWindow();
        if (parentWindow == null)
        {
            SetStatus("Error: Cannot show save dialog (no parent window found).", StatusMessageType.Error);
            System.Diagnostics.Debug.WriteLine(StatusText);
            return; // Can't proceed without a parent window
        }
        // 2. Access Avalonia's StorageProvider to show the dialog.
        // This is the modern way to handle file operations in Avalonia.
        IStorageProvider storageProvider = parentWindow.StorageProvider;
        if (!storageProvider.CanSave) // Check if saving is even possible on the current platform
        {
            SetStatus("Error: Saving files is not supported on this platform.", StatusMessageType.Error);
            System.Diagnostics.Debug.WriteLine(StatusText);
            return;
        }
        // 3. Configure the "Save File" dialog.
        // We'll tell it what kind of file we're saving (a ".pw" file).
        var pwFileType = new FilePickerFileType("PixelWallE Script")
        {
            Patterns = new[] { "*.pw" } // Users will see "*.pw" in the file type dropdown
        };

        FilePickerSaveOptions saveOptions = new FilePickerSaveOptions
        {
            Title = "Save Wall-E Script",              // Title of the dialog window
            SuggestedFileName = "my_walle_script",     // Default name offered to the user
            DefaultExtension = ".pw",                  // Automatically add ".pw" if user doesn't type it
            FileTypeChoices = new[] { pwFileType }     // Show our ".pw" file type
        };
        // 4. Show the dialog and get the user's chosen file.
        IStorageFile? selectedFile = await storageProvider.SaveFilePickerAsync(saveOptions);

        // 5. If the user selected a file (didn't cancel):
        if (selectedFile != null)
        {
            SetStatus($"Saving to {selectedFile.Name}...", StatusMessageType.Info);
            try
            {
                // Open the selected file for writing.
                await using (Stream stream = await selectedFile.OpenWriteAsync()) // Get a stream to write data
                await using (StreamWriter writer = new StreamWriter(stream))     // Use a StreamWriter for easily writing text
                {
                    // Write the actual code from our CodeText property to the file.
                    await writer.WriteAsync(this.CodeText);
                }
                SetStatus($"Successfully saved to {selectedFile.Name}.", StatusMessageType.Success);
                System.Diagnostics.Debug.WriteLine(StatusText);
            }
            catch (Exception ex)
            {
                // If anything went wrong during saving (e.g., disk full, no permission).
                SetStatus($"Error saving file: {ex.Message}.", StatusMessageType.Error);
                System.Diagnostics.Debug.WriteLine($"{StatusText}\n{ex.StackTrace}");
            }
        }
        else // User cancelled the "Save File" dialog.
        {
            SetStatus("Save operation cancelled by user.");
            System.Diagnostics.Debug.WriteLine(StatusText);
        }
    }
    private async Task LoadCodeAsync_Logic()
    {
        SetStatus("Attempting to load code...");
        // 1. Get the parent window for the dialog (same as in Save).
        Window? parentWindow = GetTopLevelWindow();
        if (parentWindow == null)
        {
            SetStatus("Error: Cannot show load dialog (no parent window found).", StatusMessageType.Error);
            System.Diagnostics.Debug.WriteLine(StatusText);
            return;
        }

        // 2. Access Avalonia's StorageProvider.
        IStorageProvider storageProvider = parentWindow.StorageProvider;
        if (!storageProvider.CanOpen) // Check if opening files is supported.
        {
            SetStatus("Error: Opening files is not supported on this platform.", StatusMessageType.Error);
            System.Diagnostics.Debug.WriteLine(StatusText);
            return;
        }

        // 3. Configure the "Open File" dialog.
        var pwFileType = new FilePickerFileType("PixelWallE Script")
        {
            Patterns = new[] { "*.pw" } // Show "*.pw" files
        };
        var allFilesType = new FilePickerFileType("All Files")
        {
            Patterns = new[] { "*.*" } // Also allow seeing all file types
        };

        FilePickerOpenOptions openOptions = new FilePickerOpenOptions
        {
            Title = "Load Wall-E Script",
            AllowMultiple = false, // We only want to load one file at a time.
            FileTypeFilter = new[] { pwFileType, allFilesType } // Let user choose between ".pw" and "All files"
        };

        // 4. Show the dialog and get the user's chosen file(s).
        IReadOnlyList<IStorageFile> selectedFiles = await storageProvider.OpenFilePickerAsync(openOptions);

        // 5. If the user selected at least one file:
        if (selectedFiles.Count >= 1)
        {
            IStorageFile fileToLoad = selectedFiles[0]; // Get the first (and only) selected file.
            SetStatus("Loading from {fileToLoad.Name}...");

            try
            {
                string fileContent;
                // Open the selected file for reading.
                await using (Stream stream = await fileToLoad.OpenReadAsync())
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Read the entire content of the file into a string.
                    fileContent = await reader.ReadToEndAsync();
                }

                // IMPORTANT: Update our ViewModel's CodeText property.
                this.CodeText = fileContent;

                // IMPORTANT: Raise the event to notify the View (MainWindow.axaml.cs)
                CodeHasBeenLoaded?.Invoke(fileContent);
                SetStatus($"Successfully loaded from {fileToLoad.Name}.", StatusMessageType.Success);
                System.Diagnostics.Debug.WriteLine(StatusText);
            }
            catch (Exception ex)
            {
                // If anything went wrong during loading
                SetStatus($"Error loading file: {ex.Message}", StatusMessageType.Error);
                System.Diagnostics.Debug.WriteLine($"{StatusText}\n{ex.StackTrace}");
            }
        }
        else // User cancelled the "Open File" dialog.
        {
            SetStatus($"Load operation cancelled by user.");
            System.Diagnostics.Debug.WriteLine(StatusText);
        }
    }

    [RelayCommand]
    private void ApplyResize()
    {
        PixelCanvas = new PixelCanvas(CanvasSize, CanvasSize);
        // CRUCIAL: Create a NEW WallE instance tied to the NEW canvas
        _wallE = new WallE(PixelCanvas);
        UpdateWallEPosition();
        SetStatus($"Canvas resized to {CanvasSize}x{CanvasSize}. Wall-E reset. Ready.");
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
        SetStatus(errorReport.ToString().Trim(), StatusMessageType.Error);
    }
    private void ReportRuntimeErrors(string phase, List<RuntimeError> errors)
    {
        var errorReport = new StringBuilder();
        errorReport.AppendLine($"{phase}:");
        foreach (var error in errors)
        {
            errorReport.AppendLine($"- {error}");
            Debug.WriteLine(error.ToString());
        }
        SetStatus(errorReport.ToString().Trim(), StatusMessageType.Error);
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
        (ProgramNode source, List<ParsingError> parserErrors) = parser.Parse();

        if (parserErrors.Any())
        {
            ReportErrors("Parser Error(s)", parserErrors);
            return; // Stop if parsing failed
        }

        if (!source.Statements.Any() && tokens.Any(t => t.Type != TokenType.EOF))
        {
            // This case might indicate the parser failed silently or recovered poorly.
            if (!parserErrors.Any()) // If no specific errors were reported, add a generic one
            {
                // StatusText = "Parsing completed, but no valid commands found.";
                Debug.WriteLine("Parsing completed, but no valid commands found.");
            }
            return;
        }


        Debug.WriteLine("--- Parsing Successful ---");
        // 4. Interpretation (Execution)
        SetStatus("Executing...");
        var interpreter = new Interpreter(_pixelCanvas, _wallE);

        RuntimeError? runtimeError = interpreter.Run(source);

        if (runtimeError != null)
        {

            ReportRuntimeErrors("Runtime Error", new List<RuntimeError> { runtimeError });
            return;

            // Status text is already set by ReportErrors
        }
        else
        {
            SetStatus("Execution finished successfully.", StatusMessageType.Success);
            PixelCanvas.NotifyChanged(); // Force UI update
            UpdateWallEPosition();      // Update Wall-E pos in VM if displayed
        }
        Debug.WriteLine("--- Code Execution Finished ---");
    }
}