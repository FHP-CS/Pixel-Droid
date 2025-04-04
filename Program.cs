
using Spectre.Console;
var canvas = new PixelCanvas(30, 30);
var WallE = new WallE();
var parser = new CommandParser();

List<Button> buttons = Button.GetButtons();

while (true)
{
    Menu.MainMenu(canvas, WallE, buttons);

}



// while (true)
// {
//     canvas.Render();
//     string intput = AnsiConsole.Ask<string>("[blue]Command:[/] ");
//     if (intput.ToLower() == "exit") break;

//     try
//     {
//         parser.Execute(intput, WallE, canvas);
//     }
//     catch (Exception ex)
//     {
//         AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
//         Console.ReadKey(true);
//     }
// }