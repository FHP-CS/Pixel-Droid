using Spectre.Console;
public class TextEditor
{
    public List<string> _lines = new List<string>();
    public int _currentLine = 0;
    public int _cursorPosition = 0;

    public string Run(PixelCanvas canvas)
    {
        _lines.Add(""); //line 0 empty
        ConsoleKeyInfo key;
        do
        {
            AnsiConsole.Clear();
            canvas.Render();
            RenderEditor();

            key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (_currentLine > 0)
                    {
                        _currentLine--;
                        _cursorPosition = Math.Min(_cursorPosition, _lines[_currentLine].Length);
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (_currentLine < _lines.Count - 1)
                    {
                        _currentLine++;
                        _cursorPosition = Math.Min(_cursorPosition, _lines[_currentLine].Length);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (_cursorPosition > 0) _cursorPosition--;
                    break;

                case ConsoleKey.RightArrow:
                    if (_cursorPosition < _lines[_currentLine].Length) _cursorPosition++;
                    break;

                case ConsoleKey.Enter:
                    _lines.Insert(_currentLine + 1, "");//insert new line
                    _currentLine++;
                    _cursorPosition = 0;
                    break;

                case ConsoleKey.Backspace:
                    if (_cursorPosition > 0)
                    {
                        _lines[_currentLine] = _lines[_currentLine].Remove(_cursorPosition - 1, 1);
                        _cursorPosition--;
                    }
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        _lines[_currentLine] = _lines[_currentLine].Insert(_cursorPosition, key.KeyChar.ToString());
                        _cursorPosition++;
                    }
                    break;

            }
        } while (key.Key != ConsoleKey.Escape);

        return string.Join("\n", _lines);
    }
    private void RenderEditor()
    {
        var panel = new Panel("Code Editor(.pw) [grey](ESC to exit)[/]")
        .BorderColor(Color.Blue);
        AnsiConsole.Write(panel);


        for (int i = 0; i < _lines.Count; i++)
        {
            string line = _lines[i];
            if (i == _currentLine)
            {
                //resaltar linea y cursor
                line = line.Insert(_cursorPosition, "[reverse on] [/]");
                AnsiConsole.MarkupLine($"[green]{_currentLine} {line} [/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"{_currentLine} {line}");
            }
        }

        AnsiConsole.Markup("[grey] Arrows: Navegate   | Enter: new line  | ESC: save[/]");
    }
}