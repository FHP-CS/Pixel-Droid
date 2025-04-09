using Avalonia.Controls;
using Avalonia.Media;
using System;
// namespace Pixel_Wall_E.Models.CanvasModel
// {
//     public class PixelCanvas
//     {
    //     private readonly ScaledCanvas _canvas;
    //     private int _canvasHeight;
    //     private int _canvasWidth;

    //     public PixelCanvas(ScaledCanvas canvas, int initialWidth, int initialHeight)//constructor
    //     {
    //         _canvas = canvas;
    //         _canvasWidth = initialWidth;
    //         _canvasHeight = initialHeight;
    //     }
    //     //methods
    //     public void ResizeCanvas(int newWidth, int newHeight)
    //     {
    //         _canvas.Children.Clear();
    //         _canvasWidth = newWidth;
    //         _canvasHeight = newHeight;
    //     }
    //     public void InitializeCanvas()
    //     {
    //         for (int x = 0; x < _canvasWidth; x++)
    //         {
    //             for (int y = 0; y < _canvasHeight; y++)
    //             {
    //                 DrawPixel(x, y, "White", 1);
    //             }
    //         }
    //     }
    //     public void ClearCanvas()
    //     {
    //         _canvas.Children.Clear();
    //         InitializeCanvas();
    //     }
    //     public void DrawPixel(int x, int y, string color, int size)
    //     {
    //         if (x < 0 || x >= _canvasWidth || y < 0 || y >= _canvasHeight) return;
    //         var rect = new Avalonia.Controls.Shapes.Rectangle
    //         {
    //             Width = size,
    //             Height = size,
    //             Fill = new SolidColorBrush(Color.Parse(color)),
    //             [Canvas.LeftProperty] = x * size,
    //             [Canvas.TopProperty] = y * size
    //         };
    //         _canvas.Children.Add(rect);
    //     }
    //     public void DrawLine(int startX, int startY, int dirX, int dirY, int distance, string color, int size)
    //     {
    //         for (int i = 0; i <= distance; i++)
    //         {
    //             var x = startX + dirX * i;
    //             var y = startY + dirY * i;

    //             DrawPixel(x, y, color, size);
    //         }
    //             var finalX = startX + dirX * distance;
    //             var finalY = startY + dirY * distance;
    //         WallE.State(finalX, finalY);
    //     }
    //     public void DrawCircle()
    //     {}
    //     public void DrawRectangle()
    //     {}
    //     public int GetCanvasWidth() => _canvasWidth;
    //     public int GetCanvasHeight() => _canvasHeight;




    // }
// }