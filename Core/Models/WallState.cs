namespace Pixel_Wall_E.Models
{
    public static class WallE
    {
        public static int X { get; set; }
        public static int Y { get; set; }
        public static string BrushColor { get; set; } = "Transparent";
        public static bool IsSpawned = false;
        public static int BrushSize { get; set; } = 1;

        public static void State(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }
        public static void SetBrushColor(string color)
        {
            BrushColor = color;
        }
        public static void SetBrushSize(int size)
        {
            BrushSize = size % 2 == 0 ? size - 1 : size; //size impar
        }
    }
}