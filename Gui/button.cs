using Spectre.Console;

public class Button
{
    public Spectre.Console.Table _button = new Table();
    public string Icon { get; set; }
    public string Info { get; set; }
    public string ShinyInfo { get; set; }

    public static int selectedButton = 1
    ;


    public Button(string icon, string info, string shinyInfo)
    {
        Icon = icon;
        Info = info;
        ShinyInfo = shinyInfo;
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
        var Run = new Run();
        var files = new Files();


        buttons.Add(openterminal);
        buttons.Add(Run);
        buttons.Add(files);

        return buttons;
    }



}
public class Terminal : Button
{
    public Terminal() : base("💻", "Dashboard", "[#84CFDB]Das[/][#97D6E1]hb[/][#B0E7F0]oa[/][#C1F6FF]rd[/]") { }
    public override void action(WallE robot, PixelCanvas canvas)
    {
        var editor = new TextEditor();
        var parser = new CommandParser();

        string code = editor.Run(TextEditor._lines);
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
    public Files() : base("[blue]📁[/]", "FileManager", "[#84CFDB]Fil[/][#97D6E1]e Ma[/][#B0E7F0]nag[/][#C1F6FF]er[/]") { }
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
public class Run : Button
{
    public Run() : base("[blue]▶ [/]", "Run", "[#84CFDB]R[/][#97D6E1]u[/][#B0E7F0]n[/]") { }
    public override void action(WallE robot, PixelCanvas canvas)
    {
        var parser = new CommandParser();
        parser.ExecuteBatch(string.Join("\n", TextEditor._lines), robot, canvas);


    }

}