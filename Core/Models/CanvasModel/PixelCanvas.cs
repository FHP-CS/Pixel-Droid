using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Input;
using System;

namespace PixelDroid
{
    public class PixelCanvas : Control
    {
        public Color[][] _pixels;

        private Color _currentDrawColor = Colors.Black; // Color de dibujo actual
        private int _currentPenSize = 1; // Tamaño del pincel actual
        public static readonly StyledProperty<int> CanvasWidthProperty =
            StyledProperty<int>.Register<PixelCanvas, int>(nameof(CanvasWidth), 10);

        public static readonly StyledProperty<int> CanvasHeightProperty =
            StyledProperty<int>.Register<PixelCanvas, int>(nameof(CanvasHeight), 10);

        public int CanvasWidth
        {
            get => GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }

        public int CanvasHeight
        {
            get => GetValue(CanvasHeightProperty);
            set => SetValue(CanvasHeightProperty, value);
        }
        public PixelCanvas()
        {
            // Inicializa _pixels con un valor no nulo, aunque sea un valor por defecto.
            _pixels = new Color[0][]; // Inicializar con un array vacío (puedes ajustarlo según tus necesidades)
            CanvasWidth = 0;
            CanvasHeight = 0;
        }

        public void InitializeCanvas(int width, int height)
        {
            CanvasWidth = width;
            CanvasHeight = height;
            _pixels = new Color[height][];
            for (int i = 0; i < height; i++)
            {
                _pixels[i] = new Color[width];
                for (int j = 0; j < width; j++)
                {
                    _pixels[i][j] = Colors.White;
                }
            }
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (_pixels == null) return;

            double pixelWidth = Bounds.Width / CanvasWidth;
            double pixelHeight = Bounds.Height / CanvasHeight;

            for (int y = 0; y < CanvasHeight; y++)
            {
                for (int x = 0; x < CanvasWidth; x++)
                {
                    var pixelColor = _pixels[y][x];
                    var rect = new Rect(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight);
                    context.FillRectangle(new SolidColorBrush(pixelColor), rect);
                }
            }
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color, int penSize)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawPixel(x0, y0, color, penSize);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            InvalidateVisual(); // Invalida el control para que se redibuje con la línea
        }

        private void DrawPixel(int x, int y, Color color, int penSize)
        {
            if (x < 0 || x >= CanvasWidth || y < 0 || y >= CanvasHeight) return;

            if (penSize == 1)
            {
                _pixels[y][x] = color;
            }
            else
            {
                // Dibujar un cuadrado/círculo de penSize alrededor del píxel
                int offset = (penSize - 1) / 2;
                for (int i = -offset; i <= offset; i++)
                {
                    for (int j = -offset; j <= offset; j++)
                    {
                        int drawX = x + i;
                        int drawY = y + j;
                        if (drawX >= 0 && drawX < CanvasWidth && drawY >= 0 && drawY < CanvasHeight)
                        {
                            _pixels[drawY][drawX] = color;
                        }
                    }
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var point = e.GetCurrentPoint(this);
            int x = (int)(point.Position.X / (Bounds.Width / CanvasWidth));
            int y = (int)(point.Position.Y / (Bounds.Height / CanvasHeight));

            if (x >= 0 && x < CanvasWidth && y >= 0 && y < CanvasHeight)
            {
                DrawPixel(x, y, _currentDrawColor, _currentPenSize);
                InvalidateVisual(); // Redibujar el canvas
            }
        }

        // Métodos para cambiar el color y el tamaño del pincel
        public void SetDrawColor(Color color)
        {
            _currentDrawColor = color;
        }

        public void SetPenSize(int size)
        {
            _currentPenSize = size;
        }
    }
}