using Spectre.Console;
public class ColorPixels
{
    private static readonly Dictionary<string, Color> _colorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Red"] = Color.Red,
        ["Blue"] = Color.Blue,
        ["Green"] = Color.Green,
        ["Yellow"] = Color.Yellow,
        ["Orange"] = Color.Orange1,
        ["Purple"] = Color.Purple,
        ["Black"] = Color.Black,
        ["White"] = Color.White,
        ["Transparent"] = Color.Default
    };
    public static Color GetColor(string colorName)
    {
        if(_colorMap.TryGetValue(colorName, out var color))
        {
            System.Console.WriteLine($"setted color {color}");
            Console.ReadKey();
            return color;
        }
        return Color.Default;
    }
}