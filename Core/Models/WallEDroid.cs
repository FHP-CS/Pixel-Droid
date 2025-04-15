using Avalonia;
using Avalonia.Media;
using PixelDroid;
namespace Pixel_Droid.Models
{
    public class WallE
    {
        public  int X { get; set; }
        public  int Y { get; set; }
        public  Color BrushColor { get; set; } = Colors.Transparent;
        public  bool IsSpawned = false;
        public  int BrushSize { get; set; } = 1;
        public WallE(int x = 0,int y =0 )
        {
            X = x;
            Y = y;
        }

        public void State(int x = 0, int y = 0 )
        {
            X = x;
            Y = y;
            
        }
        public void SetBrushColor(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "red": BrushColor = Colors.Red; break;
                case "blue": BrushColor = Colors.Blue; break;
                case "green": BrushColor = Colors.Green; break;
                case "yellow": BrushColor = Colors.Yellow; break;
                case "orange": BrushColor = Colors.Orange; break;
                case "purple": BrushColor = Colors.Purple; break;
                case "black": BrushColor = Colors.Black; break;
                case "white": BrushColor = Colors.White; break;
                // ... (Otros colores)
                default: BrushColor = Colors.Transparent; break;
            }
        }
    
        public void SetBrushSize(int size)
        {
            BrushSize = size % 2 == 0 ? size - 1 : size; //size impar
        }
    }
}