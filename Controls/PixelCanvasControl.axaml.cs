// Controls/PixelCanvasControl.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using PixelWallE.Models; // Your models namespace
using System;

namespace PixelWallE.Controls;

public partial class PixelCanvasControl : UserControl
{
    // DependencyProperty to hold the canvas data
    public static readonly StyledProperty<PixelCanvas?> CanvasDataProperty =
        AvaloniaProperty.Register<PixelCanvasControl, PixelCanvas?>(nameof(CanvasData));

    public PixelCanvas? CanvasData
    {
        get => GetValue(CanvasDataProperty);
        set => SetValue(CanvasDataProperty, value);
    }

    // Brushes cache (optional optimization)

    static PixelCanvasControl()
    {
        // Re-render when CanvasData changes
        AffectsRender<PixelCanvasControl>(CanvasDataProperty);
    }

    public PixelCanvasControl()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CanvasDataProperty)
        {
            // Unsubscribe from old canvas events, subscribe to new ones
            if (change.OldValue is PixelCanvas oldCanvas)
            {
                oldCanvas.CanvasInvalidated -= OnCanvasInvalidated;
            }
            if (change.NewValue is PixelCanvas newCanvas)
            {
                newCanvas.CanvasInvalidated += OnCanvasInvalidated;
            }
            // Trigger a render when the canvas reference changes
            InvalidateVisual();
        }
    }

    // Handler for the canvas model's notification event
    private void OnCanvasInvalidated(object? sender, EventArgs e)
    {
        // Ensure the redraw happens on the UI thread
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }


    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var canvas = CanvasData;
        if (canvas == null || canvas.Width <= 0 || canvas.Height <= 0 || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            // Handle empty state if needed
            return;
        }

        double pixelWidth = Bounds.Width / canvas.Width;
        double pixelHeight = Bounds.Height / canvas.Height;

        var pixels = canvas.GetPixelsData(); // Get direct access

        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < canvas.Width; x++)
            {
                Color color = pixels[x, y];

                // Optimization: Don't draw fully transparent pixels.
                // We DO want to draw White pixels for the background.
                if (color.A == 0) // Check Alpha channel for transparency
                    continue;

                // **** Create a NEW brush for each pixel ****
                // This ensures each DrawRectangle command gets a brush
                // instance holding the correct color at the time of queuing.
                var pixelBrush = new SolidColorBrush(color);

                var rect = new Rect(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight);
                context.DrawRectangle(pixelBrush, null, rect); // Use the new brush
            }
        }

        // Optional: Draw Grid Lines

        if (pixelWidth > 3 && pixelHeight > 3) // Only draw grid if pixels are large enough
        {
            var gridPen = new Pen(Brushes.LightGray, 0.5);
            for (int x = 0; x <= canvas.Width; x++)
            {
                context.DrawLine(gridPen, new Point(x * pixelWidth, 0), new Point(x * pixelWidth, Bounds.Height));
            }
            for (int y = 0; y <= canvas.Height; y++)
            {
                context.DrawLine(gridPen, new Point(0, y * pixelHeight), new Point(Bounds.Width, y * pixelHeight));
            }
        }

    }
}