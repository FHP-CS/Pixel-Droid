using Spectre.Console;
public static class Data
{
    public static string ProgramName = "[bold white]*[/][bold blue] PixelDroid [/][bold white]*[/]";
    public static Table Commands = new Table()
    .AddColumn(new TableColumn("Spawn(x,y) [grey]spawn Wall-E[/]\n\nResize(x,x) [grey] new size[/]\n\nClear [grey]clear all[/]"))
    .Collapse();
}