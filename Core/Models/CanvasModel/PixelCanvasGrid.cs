using Avalonia.Controls;
using Avalonia.Media;
using System;
namespace Pixel_Wall_E.Models.CanvasModel
{
    public class PixelCanvasGrid
    {
        private readonly Canvas _canvas;
        public PixelCanvasGrid(Canvas canvas)
        {
            _canvas = canvas;
        }
        public void DrawGrid(int width, int height)
        {
            _canvas.Children.Clear();//clear the canvas first

            int PixelsSize = Math.Max(1, Math.Min(width, height) / 50); //Formula for the pixels size, inversamente proporcional al tamanio

            for (int x = 0; x < width; x += PixelsSize)
            {
                var verticalLine = new Avalonia.Controls.Shapes.Line
                {
                    StartPoint = new Avalonia.Point(x, 0),
                    EndPoint = new Avalonia.Point(x, height),
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };
                _canvas.Children.Add(verticalLine);
            }
            for (int y = 0; y < height; y++)
            {
                var horizontalLine = new Avalonia.Controls.Shapes.Line
                {
                    StartPoint = new Avalonia.Point(0, y),
                    EndPoint = new Avalonia.Point(width, y),
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };
                _canvas.Children.Add(horizontalLine);
            }
        }
    }
}