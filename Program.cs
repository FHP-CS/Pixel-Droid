
using Spectre.Console;
var canvas = new PixelCanvas(30, 30);
var WallE = new WallE();
var parser = new CommandParser();

List<Button> buttons = Button.GetButtons();

while (true)
{
    Menu.MainMenu(canvas, WallE, buttons);

}
