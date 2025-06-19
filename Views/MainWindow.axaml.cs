using System; // For EventArgs
using Avalonia.Controls;
using Avalonia.Interactivity; // Required for RoutedEventArgs
using Avalonia.Input;       // Required for KeyEventArgs, TextInputEventArgs etc.
using AvaloniaEdit;         // Core AvaloniaEdit namespace
using AvaloniaEdit.CodeCompletion; // Namespace for CompletionWindow
using AvaloniaEdit.Document;       // Namespace for TextDocument, ISegment, TextUtilities
using AvaloniaEdit.Editing;        // Namespace for TextArea, CaretPositioningMode, LogicalDirection
using PixelWallE.ViewModels; // Your ViewModel namespace
using System.Collections.Generic;
using System.Linq; // For LINQ methods like Any, Where, Select, All
using AvaloniaEdit.Rendering; // Namespace for LineNumberMargin
using Avalonia.Media;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage; // For the new FilePicker API
using TextMateSharp.Grammars;
using AvaloniaEdit.TextMate;        // Needed for Brushes.Transparent


namespace PixelWallE.Views;

public partial class MainWindow : Window
{
    private TextEditor? _codeEditor; // Field to hold reference to the editor
    private MainWindowViewModel? _viewModel; // Field to hold reference to ViewModel
    private CompletionWindow? _completionWindow; // The active completion window

    // List of keywords/elements for autocompletion
    // TODO: Enhance this list or load dynamically
    private readonly List<string> _keywords = new List<string>
    {
        // Commands
        "Spawn(", "Color(", "Size(", "DrawLine(",
        "DrawCircle(", "DrawRectangle(", "Fill()",
        "GoTo [",
        "[]",
        // Functions (remove parentheses for keyword completion)
        "GetActualX()", "GetActualY()", "GetCanvasSize()", "GetColorCount(",
        "IsBrushColor(", "IsBrushSize(", "IsCanvasColor(",
        "()", ")",
        // Colors (could be separate category later)
        "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent", "LightBlue", "DarkBlue", "Violet", "Brown", "Gray", "Grey", "Pink",
        @"""Red""" , @"""Blue""", @"""Green""", @"""Yellow""", @"""Orange""", @"""Purple""", @"""Black""", @"""White""", @"""Transparent"""
    };

    public MainWindow()
    {
        InitializeComponent();

        _codeEditor = this.FindControl<TextEditor>("CodeEditor");
        var _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var _textMateInstallation = _codeEditor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));

        if (_codeEditor != null)
        {
            this.Loaded += MainWindow_Loaded; // Setup happens in Loaded
            _codeEditor.Document.TextChanged += CodeEditor_TextChanged;
            _codeEditor.TextArea.TextEntered += TextArea_TextEntered;
            _codeEditor.TextArea.TextEntering += TextArea_TextEntering;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Error: CodeEditor control not found!");
        }

    }
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
    private void MaximizeWindow(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
            this.WindowState = WindowState.Normal;
        else
            this.WindowState = WindowState.Maximized;
    }
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as MainWindowViewModel;

        if (_viewModel != null && _codeEditor != null)
        {
            // --- Initial Text Load ---
            _codeEditor.Document.TextChanged -= CodeEditor_TextChanged;
            _codeEditor.Document.Text = _viewModel.CodeText ?? "";
            _codeEditor.Document.TextChanged += CodeEditor_TextChanged;

            // Subscribe to the ViewModel's event.
                _viewModel.CodeHasBeenLoaded -= ViewModel_CodeHasBeenLoaded; // Defensive unsubscribe
                _viewModel.CodeHasBeenLoaded += ViewModel_CodeHasBeenLoaded; // Subscribe
            // --- Add Spacer Margin ---
            AddSpacerToLeftMargin(_codeEditor.TextArea, 10); // Add a 10px spacer
        }
    }
    private void ViewModel_CodeHasBeenLoaded(string newCode)
    {
        if (_codeEditor != null)
        {
            // Temporarily unsubscribe from the editor's TextChanged event
            // to prevent it from immediately re-updating the ViewModel
            // while we are programmatically setting the editor's text.
            _codeEditor.Document.TextChanged -= CodeEditor_TextChanged;

            _codeEditor.Document.Text = newCode; // Set the TextEditor's content

            // Re-subscribe so that user typing will again update the ViewModel.
            _codeEditor.Document.TextChanged += CodeEditor_TextChanged;
        }
    }
    private void AddSpacerToLeftMargin(TextArea textArea, double spacerWidth)
    {
        if (textArea == null) return;

        // Find the LineNumberMargin
        var lineNumberMargin = textArea.LeftMargins.OfType<LineNumberMargin>().FirstOrDefault();

        if (lineNumberMargin != null)
        {
            // Check if spacer already exists (to avoid adding multiple times on hot reload etc.)
            bool spacerExists = textArea.LeftMargins
                                      .OfType<Control>()
                                      .Any(c => c.Name == "LineNumberSpacer");

            if (!spacerExists)
            {
                var spacer = new Control
                {
                    Name = "LineNumberSpacer", // Give it a name for identification
                    Width = spacerWidth,
                };

                // Find the index of the LineNumberMargin and insert the spacer after it
                int index = textArea.LeftMargins.IndexOf(lineNumberMargin);
                if (index != -1)
                {
                    textArea.LeftMargins.Insert(index + 1, spacer);
                    System.Diagnostics.Debug.WriteLine($"Spacer added after LineNumberMargin at index {index + 1}");
                }
                else
                {
                    // Fallback: Add at the end if index wasn't found (shouldn't happen often)
                    textArea.LeftMargins.Add(spacer);
                    System.Diagnostics.Debug.WriteLine("Spacer added (LineNumberMargin index not found, added at end)");
                }
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("LineNumberMargin not found in LeftMargins.");
        }
    }

    // Update ViewModel when text changes in the editor (Corrected signature)
    private void CodeEditor_TextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null && _codeEditor != null)
        {
            _viewModel.CodeText = _codeEditor.Document.Text;
        }
    }
    // IMPORTANT: Unsubscribe from events when the window is closed.
        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CodeHasBeenLoaded -= ViewModel_CodeHasBeenLoaded; // Unsubscribe
            }
            base.OnClosed(e);
        }
    // --- Autocompletion Logic ---

    private void TextArea_TextEntering(object? sender, TextInputEventArgs e)
    {
        // --- Add Null Check ---
        if (_codeEditor == null) return;
        // ---------------------

        // Close completion window if a non-identifier character is typed
        if (e.Text != null && e.Text.Length > 0 && _completionWindow != null)
        {
            // Allow '-' within potential identifiers/labels but close on others like '(', ')', ',', space etc.
            if (!char.IsLetterOrDigit(e.Text[0]) && e.Text[0] != '-')
            {
                _completionWindow.Close();
                _completionWindow = null; // Ensure reference is cleared
            }
        }
    }

    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        // --- Add Null Check ---
        if (_codeEditor == null) return;
        // ---------------------

        if (e.Text == null || e.Text.Length == 0 || !char.IsLetter(e.Text[0]))
        {
            return;
        }

        if (_completionWindow != null) return;

        // --- Get Word and Segment using the helper method ---
        var (partialWord, completionSegment) = GetWordAndSegmentBeforeCaret(_codeEditor.TextArea);
        // --------------------------------------------------

        // Check if a valid word and segment were found
        if (string.IsNullOrWhiteSpace(partialWord) || completionSegment == null)
        {
            return; // Exit if no valid word/segment to complete
        }

        _completionWindow = new CompletionWindow(_codeEditor.TextArea);

        // --- IMPORTANTE: Establecer el segmento usando la información devuelta ---
        _completionWindow.StartOffset = completionSegment.Offset;    // Use Offset for the start
        _completionWindow.EndOffset = completionSegment.EndOffset; // EndOffset is correct (Start + Length)
                                                                   // -------------------------------------------------------------

        _completionWindow.Closed += (o, args) => _completionWindow = null;

        var data = _completionWindow.CompletionList.CompletionData;
        data.Clear();

        // Add keywords/elements that start with the typed word
        foreach (var keyword in _keywords.Where(k => k.StartsWith(partialWord, System.StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.Equals(keyword, partialWord, System.StringComparison.OrdinalIgnoreCase))
            {
                data.Add(new MyCompletionData(keyword));
            }
        }

        if (data.Any())
        {
            _completionWindow.MinWidth = 150;
            _completionWindow.Show();
        }
        else
        {
            _completionWindow.Close();
            _completionWindow = null;
        }
    }

    // Helper to get the identifier-like word the caret is currently inside or right after
    private (string Word, ISegment? Segment) GetWordAndSegmentBeforeCaret(TextArea textArea)
    {
        if (textArea?.Document == null || textArea.Caret == null)
            return (string.Empty, null);

        int offset = textArea.Caret.Offset;
        if (offset <= 0)
            return (string.Empty, null);

        // Find the start of the potential identifier word backward from the caret
        int startOffset = TextUtilities.GetNextCaretPosition(textArea.Document, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);

        if (startOffset < 0 || startOffset > offset) // Use '>' because start can be == offset if caret is at word start
            return (string.Empty, null);

        // The segment is from startOffset up to the current caret position
        int length = offset - startOffset;
        if (length <= 0) // If length is 0, no word segment found before caret
            return (string.Empty, null);

        string potentialWord = textArea.Document.GetText(startOffset, length);

        // Basic validation (allow starting with letter, contain letter/digit/dash)
        // Adjust this validation if your identifier rules change
        if (potentialWord.Length > 0 && char.IsLetter(potentialWord[0]) &&
            potentialWord.All(c => char.IsLetterOrDigit(c) || c == '-'))
        {
            // Create the segment object
            ISegment segment = new TextSegment { StartOffset = startOffset, Length = length };
            return (potentialWord, segment);
        }

        return (string.Empty, null); // Not a valid identifier format before caret
    }
}

// --- Simple Implementation of ICompletionData ---
public class MyCompletionData : ICompletionData
{
    public MyCompletionData(string text) { this.Text = text; }
    public Avalonia.Media.IImage? Image => null;
    public string Text { get; }
    public object Content => this.Text;
    public object Description => $"Complete to '{this.Text}'";
    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, System.EventArgs insertionRequestEventArgs)
    {
        // completionSegment is provided by the CompletionWindow based on the Start/End offsets we set.
        textArea.Document.Replace(completionSegment, this.Text);
    }
}