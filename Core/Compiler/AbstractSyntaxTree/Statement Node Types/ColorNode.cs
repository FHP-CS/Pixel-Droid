// Color(colorString)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class ColorNode : StatementNode
{
    public override Token Token { get; }
    public ExpressionNode ColorExpression { get; } // Could be a StringLiteralNode or a variable holding a color string

    public ColorNode(Token token, ExpressionNode colorExpression)
    {
        Token = token;
        ColorExpression = colorExpression;
    }
    public override string ToString() => $"Color({ColorExpression})";
    public override void Execute(Interpreter Interpreter)
    {
        if (ColorExpression.Evaluate(Interpreter) is string color)
            Interpreter.WallEInstance.SetColor(color);
        else
            throw new RuntimeError($"Argument ({ColorExpression} must be a string", Token);


    }
}