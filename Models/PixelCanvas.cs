using Spectre.Console;
using Spectre.Console.Rendering;
public class PixelCanvas
{
    private Color[,] _pixels;
    public int _baseCellSize;

    public int Width { get; set; }
    public int Height { get; set; }

    public PixelCanvas(int width, int height, int baseCellSize = 4)
    {
        Width = width;
        Height = height;
        _baseCellSize = Math.Max(2, baseCellSize);//display pixels
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
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            _pixels[x, y] = color;
    }
    public Table Render()
    {
        var backgroundGrey = new Style(background: Color.Grey23);

        var table = new Table()
        .Border(TableBorder.None)
        .BorderColor(Color.Grey23);

        table.AddColumn(new TableColumn("").RightAligned());//empty first column for coords
        //Columns
        for (int x = 0; x < Width; x++)
        {
            table.AddColumn(new TableColumn($"{x}").Centered());
        }
        //Rows
        for (int y = 0; y < Height; y++)
        {
            var coordYcell = new Text($"{y}", backgroundGrey);
            var rowCells = new List<IRenderable> { coordYcell };

            for (int x = 0; x < Width; x++)
            {
                var pixel = _pixels[x, y] == Color.Default
                ? new Text("   ", backgroundGrey)
                : new Text("▇▇", new Style(_pixels[x, y], Color.Grey23));
                rowCells.Add(pixel);
            }
            table.AddRow(rowCells);
        }
        return table;
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
        Refresh();
    }
    public void Refresh()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                SetPixel(x, y, _pixels[x, y]);
            }
        }
    }



}