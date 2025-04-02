
using Spectre.Console;
var canvas = new PixelCanvas(30, 30);
var WallE = new WallE();
var parser = new CommandParser();


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
canvas.Render();
parser.Compiler(WallE, canvas);