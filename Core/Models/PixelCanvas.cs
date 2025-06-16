// Models/PixelCanvas.cs
using Avalonia.Media;
using System;
using System.Diagnostics.CodeAnalysis; // Needed for MemberNotNull attribute

namespace PixelWallE.Models;

public class PixelCanvas
{
    // The field that caused the warning
    private Color[,] _pixels; // Now it will be initialized in the constructor

    public int Width { get; private set; }
    public int Height { get; private set; }

    public event EventHandler? CanvasInvalidated; // Event to notify when canvas changes

    // Constructor NOW initializes _pixels directly
    public PixelCanvas(int width, int height)
    {
        // Validate dimensions right at the start
        if (width <= 0 || height <= 0)
        {
            // Or handle differently, maybe default to a minimum size?
            throw new ArgumentOutOfRangeException(nameof(width) + "/" + nameof(height), "Canvas dimensions must be positive.");
        }

        Width = width;
        Height = height;
        // **Initialize the array here**
        _pixels = new Color[width, height];

        // Clear to the initial state (white)
        Clear(Colors.White);
        // Note: Clear calls NotifyChanged, so we don't need it here explicitly
    }

    // Method to resize AND clear the canvas
    // It now assumes _pixels already exists but needs reallocation
    public void Resize(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width) + "/" + nameof(height), "Canvas dimensions must be positive.");
        }

        // Only reallocate and clear if dimensions actually change
        if (width != Width || height != Height)
        {
            Width = width;
            Height = height;
            _pixels = new Color[width, height]; // Reallocate
            Clear(Colors.White); // Clear the new array
        }
        // If dimensions are the same, do nothing or just clear?
        // Current behaviour: only acts on size change. If you want resize(same, same)
        // to clear, call Clear(Colors.White) outside the if block.
    }

    // Clear the canvas with a specific color
    public void Clear(Color color)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _pixels[x, y] = color;
            }
        }
        NotifyChanged(); // Notify after clearing
    }

    // Set a single pixel color, handling bounds
    public bool SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _pixels[x, y] = color;
            // We will call NotifyChanged() after the *entire* drawing operation (like DrawLine)
            return true; // Pixel was set
        }
        return false; // Pixel out of bounds
    }

    // Get a single pixel color
    public Color GetPixel(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return _pixels[x, y];
        }
        // Consistent with SetPixel, out-of-bounds reads can return a default
        return Colors.Transparent; // Or throw? Transparent seems reasonable.
    }
    public int GetColorCount(Color color, int x1, int y1, int x2, int y2)
    {
        int count =0;
        if (x1 >= 0 && x1 < Width && y1 >= 0 && y1 < Height &&
           x2 >= 0 && x2 < Width && y2 >= 0 && y2 < Height)
        {
            
            if (x1 <= x2 && y1 <= y2)
            {
                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if(_pixels[i,j] == color) count++;
                    }
                }
                return count;
            }
            if( x1 <= x2 && y1 >= y2)
            {
                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j >= y2; j--)
                    {
                        if(_pixels[i,j] == color) count++;
                    }
                }
                return count;
            }
            ///
            if( x1 >= x2 && y1 >= y2)
            {
                for (int i = x1; i >= x2; i--)
                {
                    for (int j = y1; j >= y2; j--)
                    {
                        if(_pixels[i,j] == color) count++;
                    }
                }
                return count;
            }
            if( x1 >= x2 && y1 <= y2)
            {
                for (int i = x1; i >= x2; i--)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if(_pixels[i,j] == color) count++;
                    }
                }
                return count;
            }
            
        }
        return -1; //error 
    }

    // Helper to get the raw pixel data (useful for rendering)
    public Color[,] GetPixelsData()
    {
        return _pixels;
    }

    // Call this after a drawing operation completes
    public void NotifyChanged()
    {
        CanvasInvalidated?.Invoke(this, EventArgs.Empty);
    }
}