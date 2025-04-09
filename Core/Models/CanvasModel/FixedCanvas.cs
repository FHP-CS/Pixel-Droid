using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Pixel_Wall_E.Models.CanvasModel
{
    public class FixedCanvas : Control
    {
        public static readonly StyledProperty<int> LogicalWidthProperty =
            StyledProperty<int>.Register<FixedCanvas, int>(nameof(LogicalWidth), 10);

        public static readonly StyledProperty<int> LogicalHeightProperty =
            StyledProperty<int>.Register<FixedCanvas, int>(nameof(LogicalHeight), 10);

        public int LogicalWidth
        {
            get => GetValue(LogicalWidthProperty);
            set => SetValue(LogicalWidthProperty, value);
        }

        public int LogicalHeight
        {
            get => GetValue(LogicalHeightProperty);
            set => SetValue(LogicalHeightProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Tamaño físico del canvas
            double physicalWidth = Bounds.Width;
            double physicalHeight = Bounds.Height;

            // Tamaño de cada celda
            double cellWidth = physicalWidth / LogicalWidth;
            double cellHeight = physicalHeight / LogicalHeight;

            // Dibujar la cuadrícula
            DrawGrid(context, cellWidth, cellHeight);

            // Aquí puedes agregar lógica para dibujar píxeles individuales si es necesario.
        }

        private void DrawGrid(DrawingContext context, double cellWidth, double cellHeight)
        {
            var gridPen = new Pen(Brushes.Gray, 0.5);

            // Dibujar líneas verticales
            for (int x = 0; x <= LogicalWidth; x++)
            {
                double xPos = x * cellWidth;
                context.DrawLine(gridPen, new Point(xPos, 0), new Point(xPos, Bounds.Height));
            }

            // Dibujar líneas horizontales
            for (int y = 0; y <= LogicalHeight; y++)
            {
                double yPos = y * cellHeight;
                context.DrawLine(gridPen, new Point(0, yPos), new Point(Bounds.Width, yPos));
            }
        }
    }
}
