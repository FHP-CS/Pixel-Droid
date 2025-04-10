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
        public MainWindow()
        {
            InitializeComponent();
            GenerateAxisNumbers();

#if DEBUG
            this.AttachDevTools();
#endif
        }
        private void OnUpdateCanvasClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (int.TryParse(LogicalWidthInput.Text, out int logicalWidth) &&
                int.TryParse(LogicalHeightInput.Text, out int logicalHeight) &&
                logicalWidth > 0 && logicalHeight > 0)
            {
                PixelCanvasControl.LogicalWidth = logicalWidth;
                PixelCanvasControl.LogicalHeight = logicalHeight;

                // Forzar redibujado del canvas
                PixelCanvasControl.InvalidateVisual();
                GenerateAxisNumbers();
            }
        }
        // private void InitializePixelCanvas()
        // {
        //     _pixelCanvas = new PixelCanvas(PixelCanvasControl, _canvasWidth, _canvasHeight);
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
        //         _canvasWidth = newWidth;
        //         _canvasHeight = newHeight;

        //         PixelCanvasControl.CanvasWidth = newWidth;
        //         PixelCanvasControl.CanvasHeight = newHeight;

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
            double scaleFactor = baseLogicalSize / Math.Max(PixelCanvasControl.LogicalWidth, PixelCanvasControl.LogicalHeight);

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
            double cellWidth = 660.0 / PixelCanvasControl.LogicalWidth;
            double cellHeight = 670.0 / PixelCanvasControl.LogicalHeight;

            // Eje X (superior)
            for (int i = 0; i < PixelCanvasControl.LogicalWidth; i++)
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
            for (int i = PixelCanvasControl.LogicalHeight - 1; i >= 0; i--)
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
        private void ProcessCommand(string command)
        {
            // var canvasWidth = _pixelCanvas.GetCanvasWidth();
            // var canvasHeight = _pixelCanvas.GetCanvasHeight();
            // if (command.StartsWith("Spawn"))
            // {
            //     var args = ExtractArguments(command);

            //     var x = int.Parse(args[0]);
            //     var y = int.Parse(args[1]);

            //     if (x < 0 || x >= canvasWidth || y < 0 || y >= canvasHeight) throw new Exception("Wall-E out of the limits.");
            //     _pixelCanvas.DrawPixel(x, y, "Black", 1);
            // }

        }
        private string[] ExtractArguments(string command)
        {
            var startIndex = command.IndexOf('(') + 1;
            var endIndex = command.IndexOf('(');
            return command.Substring(startIndex, endIndex - startIndex).Split(',');
        }
    }

}