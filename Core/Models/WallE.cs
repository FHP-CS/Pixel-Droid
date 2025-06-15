// Models/WallE.cs
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics; // For Debug.WriteLine

namespace PixelWallE.Models;

public class WallE
{
    private readonly PixelCanvas _canvas;

    public int X { get; private set; }
    public int Y { get; private set; }
    public Color BrushColor { get; private set; }
    public int BrushSize { get; private set; } // Must be odd

    // Constructor requires the canvas it operates on
    public WallE(PixelCanvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        // Default state - Note: Spawn HAS to be the first command called by user code
        X = -1; // Indicate invalid state until Spawn is called
        Y = -1;
        BrushColor = Colors.Transparent; // Default brush color
        BrushSize = 1;                   // Default brush size
    }

    /// <summary>
    /// Initializes Wall-E's position on the canvas. Must be the first command.
    /// </summary>
    /// <param name="x">Initial X coordinate.</param>
    /// <param name="y">Initial Y coordinate.</param>
    /// <returns>True if spawn successful, false otherwise (e.g., out of bounds).</returns>
    public bool Spawn(int x, int y)
    {
        // Validate position against canvas bounds
        if (x < 0 || x >= _canvas.Width || y < 0 || y >= _canvas.Height)
        {
            Debug.WriteLine($"Error: Spawn position ({x}, {y}) is outside canvas bounds (W:{_canvas.Width}, H:{_canvas.Height}).");
            // Specification requires a runtime error. How you handle this depends
            // on your interpreter design (e.g., throw exception, set error state).
            // For now, let's just return false and log.
            return false;
        }

        X = x;
        Y = y;
        Debug.WriteLine($"Spawned Wall-E at ({X}, {Y})");
        // Spawn itself doesn't draw anything, just sets position.
        return true;
    }

    /// <summary>
    /// Sets the drawing color.
    /// </summary>
    /// <param name="colorName">Name of the color (e.g., "Red", "Blue").</param>
    /// 
    public Dictionary<string, Color> _Colors = new Dictionary<string, Color>{
        {"red",Colors.Red},
        {"blue",Colors.Blue},
        {"green",Colors.Green},
        {"yellow",Colors.Yellow},
        {"orange",Colors.Orange},
        {"purple",Colors.Purple},
        {"black",Colors.Black},
        {"white",Colors.White},
        {"transparent",Colors.Transparent},
        ///
        /// 
        {"pink",Colors.Pink},
        {"gray",Colors.Gray},
        {"grey",Colors.Gray},
        {"violet",Colors.Violet},
        {"brown",Colors.Brown},

    };
    public bool SetColor(string colorName) // We'll need a way to parse color names later
    {
        // Basic color mapping - expand this!
        string color = colorName.ToLower();
        if (!_Colors.TryGetValue(color, out Color _brushColor)) return false;

        BrushColor = _brushColor;
        Debug.WriteLine($"Set Brush Color to: {BrushColor}");
        return true;


    }
    public bool ValidPos(int x, int y)
    {
        if (x >= 0 && x <= _canvas.Height && y >= 0 && y <= _canvas.Height) return true;
        return false;
    }
    public bool Fill()
    {
        Color fillColor = BrushColor;
        Color targetColor = _canvas.GetPixel(X, Y);
        if (targetColor == fillColor) return true;

        flood(X, Y, targetColor, fillColor);
        return true;
    }

    public void flood(int x, int y, Color targetColor, Color fillColor)
    {
        Color pixColor = _canvas.GetPixel(x, y);
        if (pixColor == targetColor) _canvas.SetPixel(x, y, fillColor);
        else return;

        int[] downPixel = { x, y + 1 };
        int[] upPixel = { x, y - 1 };
        int[] rightPixel = { x + 1, y };
        int[] leftPixel = { x - 1, y };


        if (ValidPos(downPixel[0], downPixel[1])) flood(downPixel[0], downPixel[1], targetColor, fillColor);
        if (ValidPos(upPixel[0], upPixel[1])) flood(upPixel[0], upPixel[1], targetColor, fillColor);
        if (ValidPos(rightPixel[0], rightPixel[1])) flood(rightPixel[0], rightPixel[1], targetColor, fillColor);
        if (ValidPos(leftPixel[0], leftPixel[1])) flood(leftPixel[0], leftPixel[1], targetColor, fillColor);

    }

    /// <summary>
    /// Sets the brush size (thickness). Must be odd.
    /// </summary>
    /// <param name="k">Desired size.</param>
    public void SetSize(int k)
    {
        if (k <= 0) k = 1; // Minimum size is 1
        if (k % 2 == 0) k--; // If even, use k-1
        BrushSize = k;
        Debug.WriteLine($"Set Brush Size to: {BrushSize}");
    }


    /// <summary>
    /// Draws a line from the current position in the specified direction.
    /// </summary>
    /// <param name="dirX">X direction (-1, 0, 1).</param>
    /// <param name="dirY">Y direction (-1, 0, 1).</param>
    /// <param name="distance">Length of the line in pixels.</param>
    /// <returns>True if drawing happened, false otherwise (e.g., invalid direction).</returns>
    /// 
    public bool DrawCircle(int r)
    {
        int[] Vt = new int[] { X, Y }; //Vector for traslation 
        //centre
        int cx = 0;   //                     0,-r
        int cy = 0;   //                     |
        //Variables  //           -r,0  -----|-----  r,0
        double x = 0;   //                      |
        double y = -r;  //                      0,r
        if (r < 0)
        {
            Debug.WriteLine($"Error: The radius {r} is less than 0.");
            return false; // Invalid direction
        }

        while (x < -y)
        {
            double yMid = y + 0.5;
            if (x * x + yMid * yMid > r * r) y++;

            _canvas.SetPixel((int)(cx + x + Vt[0]), (int)(cy + y + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx - x + Vt[0]), (int)(cy + y + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx + x + Vt[0]), (int)(cy - y + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx - x + Vt[0]), (int)(cy - y + Vt[1]), BrushColor);

            _canvas.SetPixel((int)(cx + y + Vt[0]), (int)(cy + x + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx + y + Vt[0]), (int)(cy - x + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx - y + Vt[0]), (int)(cy + x + Vt[1]), BrushColor);
            _canvas.SetPixel((int)(cx - y + Vt[0]), (int)(cy - x + Vt[1]), BrushColor);

            x++;
        }
        return true;


    }
    public bool DrawLine(int dirX, int dirY, int distance)
    {
        // Validate direction
        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1 || (dirX == 0 && dirY == 0))
        {
            Debug.WriteLine($"Error: Invalid direction ({dirX}, {dirY}) for DrawLine.");
            return false; // Invalid direction
        }
        if (distance <= 0) return false; // Drawing a line of 0 length does nothing, successfully.
        if (BrushColor == Colors.Transparent)
        {
            // Move Wall-E without drawing
            X += dirX * distance;
            Y += dirY * distance;
            Debug.WriteLine($"Moved Wall-E (Transparent Brush) to ({X}, {Y})");
            // Note: We might need to clamp X,Y to canvas bounds if movement can go OOB
            return true;
        }

        Debug.WriteLine($"Drawing Line: Dir=({dirX},{dirY}), Dist={distance}, Start=({X},{Y}), Color={BrushColor}, Size={BrushSize}");
        // int currentX = X;
        // int currentY = Y;
        bool pixelsDrawn = false;

        // Draw points along the line
        for (int i = 0; i < distance; i++)
        {
            // Calculate the center pixel for this step
            int stepX = X + dirX * i;
            int stepY = Y + dirY * i;

            // Draw the brush square centered at (stepX, stepY)
            if (i == 0 && distance > 1)//first step 
                _canvas.SetPixel(stepX, stepY, BrushColor);
            else
            {
                pixelsDrawn |= DrawBrushAt(stepX, stepY);
            }
        }

        // Special case: The last point needs to be drawn too if distance > 0
        int finalX = X + dirX * distance;
        int finalY = Y + dirY * distance;
        // pixelsDrawn |= DrawBrushAt(finalX, finalY);


        // Update Wall-E's position to the end of the line
        X = finalX;
        Y = finalY;
        Debug.WriteLine($"Line finished. Wall-E position: ({X}, {Y})");

        if (pixelsDrawn)
        {
            _canvas.NotifyChanged(); // Notify canvas UI to update after the whole line is drawn
        }
        return true;
    }

    // Helper to draw the brush square centered at a point
    private bool DrawBrushAt(int centerX, int centerY)
    {
        if (BrushColor == Colors.Transparent) return false;

        int halfSize = (BrushSize - 1) / 2;
        bool drawn = false;

        for (int dx = -halfSize; dx <= halfSize; dx++)
        {
            for (int dy = -halfSize; dy <= halfSize; dy++)
            {
                drawn |= _canvas.SetPixel(centerX + dx, centerY + dy, BrushColor);
            }
        }
        // Small logging for brush application if needed
        // if (drawn) Debug.WriteLine($"  - Brush applied around ({centerX}, {centerY})");
        return drawn;
    }
}