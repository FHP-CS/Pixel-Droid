using Spectre.Console;
public class WallE
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsSpawned = false;
    public Color currentColor { get; set; } = Color.White;
    public int BrushSize { get; set; } = 1;

    public void Spawn(int x, int y, PixelCanvas canvas)
    {
        X = x;
        Y = y;
        System.Console.WriteLine($"wallE at {X},{Y}");
        canvas.SetPixel(X, Y, Color.DarkGoldenrod);
    }
}