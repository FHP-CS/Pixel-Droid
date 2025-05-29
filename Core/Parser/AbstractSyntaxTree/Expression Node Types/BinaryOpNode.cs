using PixelWallE.Common;
using System;
public class BinaryOpNode : ExpressionNode
{
    public ExpressionNode Left { get; }
    public Token OperatorToken { get; } // Stores the operator type (+, *, <, &&, etc.)
    public ExpressionNode Right { get; }

    public BinaryOpNode(ExpressionNode left, Token operatorToken, ExpressionNode right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }
    public override object Evaluate()
    {
        int x =  int.Parse(Left.Evaluate().ToString());
        int y = int.Parse(Right.Evaluate().ToString());
        switch (OperatorToken.Type)
        {
            case TokenType.Plus:
                return x + y;
            case TokenType.Minus:
                return x - y;
            case TokenType.Multiply:
                return x * y;
            case TokenType.Modulo:
                return x % y;
            case TokenType.Power:
                return x^y;
            case TokenType.Divide:
                if (y == 0)
                    throw new DivideByZeroException("Error: Division by zero.");
                return x / y;
            default:
                throw new Exception($"Runtime Error: Unknown operator type {OperatorToken.Type}");

        }
    }
    public override string ToString() => $"({Left} {OperatorToken.Literal} {Right})";
}