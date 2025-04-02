using Spectre.Console;
public class PixelCanvas
{
    public Spectre.Console.Canvas _canvas;
    private Color[,] _pixels;
    public int Width { get; set; }
    public int Height { get; set; }

    public PixelCanvas(int width, int height)
    {
        Width = width;
        Height = height;
        _canvas = new Spectre.Console.Canvas(width, height);//display pixels
        _pixels = new Color[width, height];//data of pixels
        Clear(); //inicialize with white board default
    }
    public void Clear()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                SetPixel(x, y, Color.White);
            }
        }
    }


    public void SetPixel(int x, int y, Color color)
    {
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1) return;
        _pixels[x, y] = color;
        _canvas.SetPixel(x, y, color);
    }
        public void Resize(int newWidth, int newHeight)
    {
        var newPixels = new Color[newWidth, newHeight];//Pixel Matrix
        //start the color matrix all white
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                newPixels[x, y] = Color.White;
            }
        }
        //Copy existing pixels
        int copyWidth = Math.Min(Width, newWidth);
        int copyHeight = Math.Min(Height, newHeight);

        for (int y = 0; y < copyHeight; y++)
        {
            for (int x = 0; x < copyWidth; x++)
            {
                newPixels[x, y] = _pixels[x, y];
            }
        }
        //refresh
        Height = newHeight;
        Width = newWidth;
        _pixels = newPixels;
        _canvas = new Spectre.Console.Canvas(newWidth, newHeight);
        Refresh();
    }
    public void Refresh()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _canvas.SetPixel(x, y, _pixels[x, y]);
            }
        }
    }

    public void Render()
    {
        var table = new Table()
        .BorderColor(Color.Blue3_1)
        .Centered();
        table.AddColumn(new TableColumn("[yellow]Commands:[/]"));
        table.AddColumn(new TableColumn(Data.ProgramName + $"                       [bold]{Width}[/]x[bold]{Height}[/]").Centered()).Expand();
        table.AddRow(Data.Commands, _canvas);
        AnsiConsole.Clear();
        AnsiConsole.Write(table);
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