using Avalonia.Diagnostics;
using Avalonia.DesignerSupport;
using Avalonia;
using Avalonia.Controls;
using Pixel_Droid.Models;
using Pixel_Droid.Models.CanvasModel;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using System;
using Avalonia.Interactivity;
using Avalonia.Layout;
using MsBox.Avalonia;
using Tmds.DBus.Protocol;
using MsBox.Avalonia.Enums;
using Avalonia.Threading;
namespace Pixel_Droid.Views
{

    
    public partial class MainWindow : Window
    {
        WallE WallE = new WallE();
        public MainWindow()
        {
            InitializeComponent();
            GenerateAxisNumbers();

#if DEBUG
            this.AttachDevTools();
#endif
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializar el PixelCanvas con las dimensiones deseadas
            MyCanvas.InitializeCanvas(50, 50); // Ejemplo: canvas de 50x50 píxeles
        }
        // Helper method to convert a string to a Color

        private void OnUpdateCanvasClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (int.TryParse(CanvasWidthInput.Text, out int canvasWidth) &&
                int.TryParse(CanvasHeightInput.Text, out int canvasHeight) &&
                canvasWidth > 0 && canvasHeight > 0)
            {
                MyCanvas.CanvasWidth = canvasWidth;
                MyCanvas.CanvasHeight = canvasHeight;

                // Forzar redibujado del canvas
                MyCanvas.InvalidateVisual();
                GenerateAxisNumbers();
            }
        }
        // private void InitializePixelCanvas()
        // {
        //     _pixelCanvas = new PixelCanvas(MyCanvas, CanvasWidth, CanvasHeight);
        // }
        private void OnLoadFile(object sender, RoutedEventArgs e)
        {
        }
        private void OnSaveFile(object sender, RoutedEventArgs e)
        {
        }
        // private async void OnResizeCanvas(object sender, RoutedEventArgs e)
        // {
        //     if (int.TryParse(CanvasWidthInput.Text, out var newWidth) && int.TryParse(CanvasHeigthInput.Text, out var newHeight) && newWidth > 0 && newHeight > 0)
        //     {
        //         CanvasWidth = newWidth;
        //         CanvasHeight = newHeight;

        //         MyCanvas.CanvasWidth = newWidth;
        //         MyCanvas.CanvasHeight = newHeight;

        //         InitializePixelCanvas();
        //         UpdateCanvasScale();
        //     }
        // }
        private double CalculateDynamicFontSize()
        {
            const int baseLogicalSize = 10; // Tamaño de referencia
            const double baseFontSize = 14; // Tamaño base para 10x10
            const double minFontSize = 6; // Mínimo tamaño legible

            // Calcular factor de escalado basado en el tamaño lógico
            double scaleFactor = baseLogicalSize / Math.Max(MyCanvas.CanvasWidth, MyCanvas.CanvasHeight);

            // Aplicar escala y ajustar límites
            return Math.Max(minFontSize, baseFontSize * scaleFactor);
        }
        private void GenerateAxisNumbers()
        {
            XAxisNumbers.Children.Clear();
            YAxisNumbers.Children.Clear();
            double fontSize = CalculateDynamicFontSize();

            Dispatcher.UIThread.InvokeAsync(() =>
        {
            double cellWidth = 660.0 / MyCanvas.CanvasWidth;
            double cellHeight = 670.0 / MyCanvas.CanvasHeight;

            // Eje X (superior)
            for (int i = 0; i < MyCanvas.CanvasWidth; i++)
            {
                XAxisNumbers.Children.Add(new TextBlock
                {
                    Text = i.ToString(),
                    Width = cellWidth,
                    FontSize = fontSize, // Tamaño dinámico
                    TextAlignment = TextAlignment.Center
                });
            }

            // Eje Y (izquierdo)
            for (int i = MyCanvas.CanvasHeight - 1; i >= 0; i--)
            {
                YAxisNumbers.Children.Insert(0, new TextBlock
                {
                    Text = i.ToString(),
                    Height = cellHeight,
                    Width = 15,
                    FontSize = fontSize, // Tamaño dinámico
                    TextAlignment = TextAlignment.Right
                });
            }
        }, DispatcherPriority.Background);
        }

        // private void OnExecuteCode(object sender, RoutedEventArgs e)
        // {
        //     var commands = CodeEditor.Text.Split('\n');
        //     foreach (var command in commands)
        //     {
        //         try
        //         {
        //             ProcessCommand(command.Trim());
        //         }
        //         catch (Exception ex)
        //         {
        //             break;
        //         }
        //     }
        // }
        public void ExecuteCommand(string command, string[] arguments)
        {
            switch (command)
            {
                case "Spawn":
                 int limitX = MyCanvas.CanvasWidth;
                 int limitY = MyCanvas.CanvasHeight;

                 int x = int.Parse(arguments[0]);
                 int y = int.Parse(arguments[1]);
                 if(x < 0 || x >= limitX || y < 0 || y >= limitY) return;

                    WallE.X = int.Parse(arguments[0]);
                    WallE.Y = int.Parse(arguments[1]);
                    // Asegúrate de validar si las coordenadas están dentro del canvas
                    break;
                case "Color":
                    WallE.SetBrushColor(arguments[0]);
                    break;
                case "Size":
                    WallE.SetBrushSize(int.Parse(arguments[0]));
                    break;
                case "DrawLine":
                    int dirX = int.Parse(arguments[0]);
                    int dirY = int.Parse(arguments[1]);
                    if(dirX != 0 && dirX != 1 && dirX != -1) return;
                    if(dirY != 0 && dirY != 1 && dirY != -1) return;
                    int distance = int.Parse(arguments[2]);

                    int x1 = WallE.X + (dirX * distance);
                    int y1 = WallE.Y + (dirY * distance);

                    MyCanvas.DrawLine(WallE.X, WallE.Y, x1, y1, WallE.BrushColor , WallE.BrushSize);

                    // Actualizar la posición de Wall-E
                    WallE.X = x1;
                    WallE.Y = y1;
                    break;
                // ... (Otros comandos)
            }
        }
        private string[] ExtractArguments(string command)
        {
            var startIndex = command.IndexOf('(') + 1;
            var endIndex = command.IndexOf('(');
            return command.Substring(startIndex, endIndex - startIndex).Split(',');
        }
    }

}