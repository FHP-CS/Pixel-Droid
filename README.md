# ✨ Pixel-Droid: Code Your Canvas! ✨

---

**Pixel-Droid** is a unique Windows application that transforms your lines of code into vibrant pixel art. Forget complex tools – with Pixel-Droid, you command the canvas directly through a simple yet powerful scripting language, bringing your digital masterpieces to life, one instruction at a time!

---

## 🚀 Features

*   **Command-Based Pixel Art:** Sculpt your images using an intuitive set of text commands.
*   **Integrated Development Environment:**
    *   **Live Canvas:** See your artwork update as Wall-E (your pixel-drawing bot) executes your code.
    *   **Smart Text Editor:** Write your `.pw` scripts with features to help you code.
    *   **Instant Feedback:** An error window helps you debug your creations on the fly.
    *   **Dynamic Canvas:** Resize your creative space as needed.
*   **Powerful Scripting Language:**
    *   **Full Control:** `Spawn` Wall-E, set `Color` and `Size` of its brush.
    *   **Vector Drawing:** Create `DrawLine`, `DrawCircle`, and `DrawRectangle` instructions.
    *   **Flood Fill:** Magically `Fill` enclosed areas with color.
    *   **Program Flow:** Implement logic with `Labels` and conditional `GoTo` statements.
    *   **Variables & Expressions:** Use variables, arithmetic (`+`, `-`, `*`, `/`, `**`, `%`), and boolean (`&&`, `||`, `==`, `<`, `>` etc.) expressions.
    *   **Built-in Functions:** Query Wall-E's state (`GetActualX`, `GetActualY`, `IsBrushColor`, `IsBrushSize`), canvas properties (`GetCanvasSize`, `GetColorCount`), and pixel data (`IsCanvasColor`).
*   **Save & Load:** Keep your scripts safe and revisit your artwork anytime using `.pw` files.

---

## 🏁 Getting Started

Ready to command your pixels? Here’s how:

### Prerequisites

1.  **.NET 9.0 SDK:** Ensure you have the .NET 9.0 SDK (or a compatible newer version) installed. You can download it from [Microsoft's .NET website](https://dotnet.microsoft.com/download/dotnet/9.0).
2.  **Avalonia UI Context:** While the project includes necessary Avalonia packages, running from source might require your development environment to be set up for Avalonia development.

### Running Pixel-Droid

The easiest way to run Pixel-Droid (once built):

1.  Navigate to the release directory: `Pixel-Droid\PixelWallE\bin\Release\net9.0\`
2.  Launch the `PixelWallE.exe` application.

---

## 🛠️ How It Works: Under the Hood

Pixel-Droid is cleverly designed with three core components working in harmony:

1.  🎨 **The "Paint" Logic (Models - `PixelCanvas.cs`, `WallE.cs`)**
    *   This is the heart of your artwork's state.
    *   `PixelCanvas`: Manages the grid of pixels, their colors, and operations like clearing or getting pixel data.
    *   `WallE`: Represents your drawing bot, keeping track of its position (X, Y), current brush color, and brush size. It contains the methods for drawing operations.

2.  🖥️ **The "Avalonia UI" (Views & Controls)**
    *   This is what you see and interact with! Built with the modern Avalonia framework.
    *   **Canvas Control (`PixelCanvasControl`):** A custom control that visually renders the `PixelCanvas` data, including Wall-E's position and the coordinate axes.
    *   **Text Editor (`AvaloniaEdit`):** Where you write your `.pw` command scripts.
    *   **Interactive Elements:** Buttons for Run, Save, Load, Resize, and a status bar/error display area.

3.  🧠 **The "Interpreter" (Compiler Core)**
    *   This is the engine that understands your `.pw` scripts and translates them into actions. It processes your code in three main stages:
        *   **Lexical Analysis (Lexer):** Scans your raw code script. Its job is to break down the stream of text into a sequence of meaningful "tokens" (like keywords `Spawn`, numbers `10`, operators `<-`, identifiers `my_var`). It will flag errors if it finds characters or sequences it doesn't recognize.
        *   **Syntax Analysis (Parser):** Takes the list of tokens from the Lexer. It checks if these tokens are arranged in a grammatically correct way according to the Pixel-Droid language rules. If the syntax is valid, the Parser builds an Abstract Syntax Tree (AST) – an internal, tree-like representation of your code's structure. It catches errors like "Expected a ')' but found 'EOL'".
        *   **Execution (Interpreter):** This stage "walks" the Abstract Syntax Tree built by the Parser. For each node in the tree (representing a command or an expression), the Interpreter performs the corresponding action – calling methods in the `WallE` or `PixelCanvas` models, managing variables, handling `GoTo` jumps, and evaluating expressions. It's here that runtime errors (like "Division by zero" or "Undefined label") are caught.

### 📜 The Pixel-Droid Language Flow

1.  You write commands in the **Text Editor**.
2.  You hit the **"Run"** button.
3.  The **Interpreter** kicks in:
    *   Lexer tokenizes your script.
    *   Parser builds an AST.
    *   Executor/Interpreter runs the AST.
4.  As the Interpreter executes commands:
    *   It updates the state of the **`WallE` model** (position, brush).
    *   It modifies the **`PixelCanvas` model** (changes pixel colors).
5.  The **Avalonia UI** (specifically `PixelCanvasControl`) observes changes in the `PixelCanvas` model (via the `CanvasInvalidated` event) and re-renders itself, showing you the updated artwork! Your `MainWindowViewModel` helps orchestrate these UI updates.

---

## 💻 The Pixel-Droid Command Language

Here's a peek at the commands and functions at your disposal:

### Core Commands

*   `Spawn(x, y)`: Places Wall-E at integer coordinates `x, y` on the canvas. *Must be the first operational command.*
*   `Color(colorName)`: Sets Wall-E's brush to a specified color. `colorName` should be a string like `"Red"`, `"Blue"`, `"Transparent"`, etc.
*   `Size(k)`: Sets Wall-E's brush size (thickness) to the odd integer `k`. If `k` is even, `k-1` is used. If `k <= 0`, size 1 is used.
*   `DrawLine(dirX, dirY, distance)`: Draws a line using the current brush from Wall-E's position for `distance` pixels in the direction specified by `dirX` and `dirY` (e.g., `(1,0)` for right, `(0,-1)` for up).
*   `DrawCircle(radius)`: Draws a circle outline/filled area (details depend on your implementation) centered at Wall-E's current position with the given `radius`.
*   `DrawRectangle(width, height)`: Draws a rectangle outline/filled area with the given `width` and `height`, typically with Wall-E's current position as a reference (e.g., top-left corner).
*   `Fill()`: Fills the enclosed area where Wall-E is currently located, using the current `BrushColor`. It starts from Wall-E's pixel and spreads to all connected pixels of the same color.

### Control Flow

*   `your_label_name`: Defines a label at this point in the code. Must be on its own line. Labels allow `GoTo` to jump to this line.
*   `GoTo [your_label_name] (condition)`: If the `condition` (a boolean expression) evaluates to true (non-zero), execution jumps to the line marked by `your_label_name`.

### Variables & Expressions

*   **Variable Assignment:** `my_variable <- arithmetic_expression_or_boolean_expression`
    *   Example: `x_pos <- GetActualX() + 10`
    *   Example: `is_red <- IsBrushColor("Red")`
*   **Arithmetic Expressions:** Use `+`, `-`, `*`, `/` (integer division), `%` (modulo), `**` (power) with integer operands. Standard operator precedence applies.
*   **Boolean Expressions:** Use `&&` (logical AND), `||` (logical OR), and comparison operators `==`, `>`, `<`, `>=`, `<=`.
    *   **Precedence Note:** In Pixel-Droid, `||` (OR) has *higher* precedence than `&&` (AND).
    *   Conditions often evaluate to `1` (true) or `0` (false).
*   **String Literals for Colors:** Used in `Color()` command and color-related functions, e.g., `"Red"`.

### Built-in Functions

*   `GetActualX()`: Returns Wall-E's current X coordinate (integer).
*   `GetActualY()`: Returns Wall-E's current Y coordinate (integer).
*   `GetBrushSize()`: Returns Wall-E's current brush size (integer).
*   `GetCanvasWidth()`: Returns the current width of the canvas (integer). (*Note: Your reference said `GetCanvasSize` which seemed to only return width, this assumes a more standard naming, or adjust if `GetCanvasSize` is the actual function.*)
*   `GetCanvasHeight()`: Returns the current height of the canvas (integer).
*   `GetColorCount(colorName, x1, y1, x2, y2)`: Returns the number of pixels of the specified `colorName` (string) within the rectangular region defined by top-left `(x1, y1)` and bottom-right `(x2, y2)`.
*   `IsBrushColor(colorName)`: Returns `1` (true) if Wall-E's current brush color matches `colorName` (string), otherwise `0` (false).
*   `IsBrushSize(size)`: Returns `1` (true) if Wall-E's current brush size matches `size` (integer), otherwise `0` (false).
*   `IsCanvasColor(colorName, xOffset, yOffset)`: Returns `1` (true) if the pixel on the canvas at `(WallE.X + xOffset, WallE.Y + yOffset)` matches `colorName` (string), otherwise `0` (false). Returns `0` if the coordinate is out of bounds.

---

## 💡 Future Enhancements & Contributing

Pixel-Droid is a fantastic foundation! Here are some ideas for the future, or areas where contributions would be awesome:

*   **Advanced Drawing Tools:** Bezier curves, arcs, polygons.
*   **More Functions:** Random number generators, math functions (sin, cos).
*   **Error Highlighting:** Integrate lexer/parser errors directly into the text editor by underlining problematic code.
*   **Syntax Highlighting:** Custom syntax highlighting for the `.pw` language in AvaloniaEdit.
*   **UI/UX Polish:** Improved dialogs, theming, more visual feedback.
*   **Performance Optimizations:** For very large canvases or complex operations.

If you're looking to add a new **operator**:
> You primarily need to consider its precedence relative to other operators. This will guide where you insert its parsing logic within the `ParseExpression` (and its helper methods like `ParseTerm`, `ParseFactor`, etc.) in your `Parser.cs`. Then, you'll add a case for it in `BinaryOpNode.Evaluate()` (or `UnaryOpNode.Evaluate()`) in your `Interpreter.cs`.

If you want to add a new **command or function**:
> 1.  Define its behavior: What should it do? What arguments does it take?
> 2.  Implement the core logic, likely as a new method in your `WallE.cs` or `PixelCanvas.cs` models.
> 3.  **Lexer:** Add a new `TokenType` if it's a new keyword. Add the keyword to the lexer's dictionary.
> 4.  **AST:** Define a new `AstNode` class for it (if it's a new statement type).
> 5.  **Parser:** Add parsing logic for the new command/function call syntax (e.g., a new `ParseMyNewCommandStatement()` method and a dispatch case in `ParseStatement()`, or a new case in `ParseCallOrPrimary()` for functions).
> 6.  **Interpreter:**
>     *   For commands: Implement the `Execute()` method for its AST node, which will call the logic you added to your models.
>     *   For functions: Add a new entry to the `_builtInFunctions` dictionary in `Interpreter.cs`, mapping the function name to its implementation.

---

Happy Pixel Crafting! 🤖🎨
