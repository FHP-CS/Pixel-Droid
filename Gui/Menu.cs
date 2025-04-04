using Spectre.Console;
public class Menu
{
    public static int cursorOn = 0;
    public static void Render(PixelCanvas canvas)
    {
        var table = new Table()
        .BorderColor(Color.Blue)
        .Centered();
        table.AddColumn(new TableColumn("[yellow]⚙[/]")).Collapse()
        .LeftAligned();
        table.AddColumn(new TableColumn(Data.ProgramName + $"                       [bold]{canvas.Width}[/]x[bold]{canvas.Height}[/]"))
        .Expand()
        .RightAligned();
        table.AddRow(Data.Commands, canvas._canvas);
        AnsiConsole.Clear();
        AnsiConsole.Write(table);
    }
    public static Table startTable(PixelCanvas canvas, List<Button> buttons)
    {
        var table = new Table()
                .BorderColor(Color.BlueViolet)
                .HeavyBorder()
                .Centered();
        table.AddColumn(new TableColumn("[yellow]⚙[/]")).Collapse();
        table.AddColumn(new TableColumn(Data.ProgramName + $"                       [bold]{canvas.Width}[/]x[bold]{canvas.Height}[/]").Centered()).Expand();
        var utils = new Table()
       .NoBorder()
       .LeftAligned()
       .AddColumn(new TableColumn(""));

        disign(utils, buttons);
        table.AddRow(utils, canvas._canvas);
        return table;
    }
    public static void MainMenu(PixelCanvas canvas, WallE robot, List<Button> buttons)
    {
        while (true)
        {
            var table2 = startTable(canvas, buttons);
            AnsiConsole.Clear();
            AnsiConsole.Write(table2);

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (i == Button.selectedButton)
                    {
                        buttons[i].action(robot, canvas);
                    }
                }
            }
            if (key.Key == ConsoleKey.DownArrow)
            {
                for (int i = 0; i < buttons.Count; i++)
                {

                    if (i == Button.selectedButton && i == buttons.Count - 1)//is in the last one
                    {
                        Button.selectedButton = 0;
                        break;
                    }
                    else if (i == Button.selectedButton)//if is not the last one
                    {

                        Button.selectedButton++;
                        break;

                    }

                }

            }

        }
    }
    public static void disign(Table t, List<Button> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            var selection = new Table();
            selection.NoBorder()
            .LeftAligned();

            if (i == Button.selectedButton)
            {
                selection.AddColumn(new TableColumn("[bold purple]>[/]"));
                selection.AddColumn(new TableColumn(b[i]._button));
                selection.AddColumn(new TableColumn($" {b[i].Info}"));



                t.AddRow(selection);

            }
            else
            {
                selection.AddColumn(new TableColumn(b[i]._button));
                selection.AddColumn(new TableColumn($" {b[i].Info}"));

                t.AddRow(selection);
            }
        }
    }

    // public void Render()
    // {
    //     var table = new Table()
    //     .BorderColor(Color.Blue3_1)
    //     .Centered();
    //     table.AddColumn(new TableColumn("[yellow]Commands:[/]"));
    //     table.AddColumn(new TableColumn(Data.ProgramName + $"                       [bold]{Width}[/]x[bold]{Height}[/]").Centered()).Expand();

    //     var table2 = new Table()
    //     .Border(TableBorder.None)
    //     .HideHeaders();
    //     for (int x=0; x < Width; x++)
    //     {
    //         table2.AddColumn(new TableColumn($"[silver]{x}[/]").Width(3).Centered());
    //     }
    //     for (int y = 0; y < Height; y++)
    //     {
    //         var row = new List<Text>();
    //         row.Add(new Text($"[silver]{y}[/]", new Style(Color.Grey)));

    //         for (int x = 0; x < Width; x++)
    //         {
    //             row.Add(new Text("■", new Style(_pixels[x,y])));
    //         }
    //     }
    //     table.AddRow(Data.Commands, table2);

    //     AnsiConsole.Clear();
    //     AnsiConsole.Write(table);
    // }
}