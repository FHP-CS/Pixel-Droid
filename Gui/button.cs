using Spectre.Console;

public class Button
{
    public Spectre.Console.Table _button = new Table();
    public string Icon { get; set; }
    public string Info { get; set; }
public static int selectedButton = 1
;


    public Button(string icon, string info)
    {
        Icon = icon;
        Info = info;
        _button.AddColumn(new TableColumn(icon));
        _button.BorderColor(Color.Blue);
        _button.RoundedBorder();

    }
    public virtual void action(WallE robot, PixelCanvas canvas)
    {

    }
    public static List<Button> GetButtons()
    {
        List<Button> buttons = new List<Button>();

        var openterminal = new Terminal();
        var files = new Files();

        buttons.Add(openterminal);
        buttons.Add(files);
        return buttons;
    }



}
public class Terminal : Button
{
    public Terminal() : base("📋", "[#33e3ff]Dashboard[/]") { }
    public override void action(WallE robot, PixelCanvas canvas)
    {
        var editor = new TextEditor();
        var parser = new CommandParser();

        string code = editor.Run();
        parser.ExecuteBatch(code, robot, canvas);
        Console.Clear();
        System.Console.WriteLine("Press Esc to go back");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            List<Button> buttons = GetButtons();
            Menu.MainMenu(canvas, robot, buttons);
        }
    }

}
public class Files : Button
{
    public Files() : base("💾", "[#33e3ff]FileManager[/]") { }
    public override void action(WallE robot, PixelCanvas canvas)
    {
        Console.Clear();
        System.Console.WriteLine("NOT IMPLEMENTED YET          Press Esc to go back");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            List<Button> buttons = GetButtons();
            Menu.MainMenu(canvas, robot, buttons);
        }
    }

}