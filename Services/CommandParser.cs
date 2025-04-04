using Spectre.Console;

public class CommandParser
{
    public void Compiler(WallE robot, PixelCanvas canvas)
    {
        AnsiConsole.MarkupLine("[blue] Console[/][bold]>[/] (escribe 'run' para ejecutar):");
        int line = 1;
        var commands = new List<string>();
        while (true)
        {
            string input = AnsiConsole.Ask<string>("[grey]" + line + "[/]  [black]>[/]  ");
            if (input.Trim().Equals("run", StringComparison.OrdinalIgnoreCase))// typed run so lets run the orders!
            {
                ExecuteBatch(string.Join("\n", commands), robot, canvas);
                commands.Clear();
            }
            else if (input.Trim().Equals("clear", StringComparison.OrdinalIgnoreCase))// forget about everything! this guy is insane!
            {
                commands.Clear();
                AnsiConsole.Clear();
            }
            else //there is no run, or clear, order, he is just typing orders
            {
                commands.Add(input);
                line++;
            }
            if (commands.Count == 0) line = 1;

        }
    }
    public void ExecuteBatch(string multiCommand, WallE robot, PixelCanvas canvas)
    {
        string[] commands = multiCommand.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);// pico en los separadores
        foreach (var cmd in commands)
        {
            Execute(cmd.Trim(), robot, canvas);
            Menu.Render(canvas);
            Thread.Sleep(200);
        }
    }
    public void Execute(string command, WallE robot, PixelCanvas canvas)
    {
        string[] parts = command.Split(new[] { '(', ')', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        string cmd = parts[0].ToLower().Trim();
        for (int i = 0; i < parts.Length; i++)
        {
            System.Console.WriteLine($"{parts[i]}");

        }
        switch (cmd)
        {
            case "spawn":
                int X = int.Parse(parts[1]);
                int Y = int.Parse(parts[2]);

                if (robot.IsSpawned)
                {
                    canvas.SetPixel(robot.X, robot.Y, robot.currentColor);/// change color where walle is
                }
                else
                {
                    robot.IsSpawned = true;
                }
                robot.Spawn(X, Y, canvas);
                break;

            case "resize":
                int newWidth = int.Parse(parts[1]);
                int newHeight = int.Parse(parts[2]);
                canvas.Resize(newWidth, newHeight);
                //adjust walle Spawn
                if (robot.IsSpawned)
                {
                    int newX = Math.Min(robot.X, newWidth - 1);
                    int newY = Math.Min(robot.Y, newHeight - 1);

                    robot.Spawn(newX, newY, canvas);
                }
                break;

            case "clear":
                canvas.Clear();
                break;

            case "color":
                robot.currentColor = ColorPixels.GetColor(parts[1].Trim('"'));
                break;

            case "drawline":
                int dirX = int.Parse(parts[1]); //direction in X
                int dirY = int.Parse(parts[2]); //direction in y
                int distance = int.Parse(parts[3]); //distance to travel
                DrawLine(robot, canvas, dirX, dirY, distance);
                break;
        }

    }
    private void DrawLine(WallE robot, PixelCanvas canvas, int dirX, int dirY, int distance)
    {
        int x0 = robot.X;
        int y0 = robot.Y;
        int x = robot.X + dirX * distance;
        int y = robot.Y + dirY * distance;
        robot.Spawn(x, y, canvas);
        for (int j = 0; j < distance; j++)
        {
            canvas.SetPixel(x0 + dirX * j, y0 + dirY * j, robot.currentColor);
        }


    }
}
