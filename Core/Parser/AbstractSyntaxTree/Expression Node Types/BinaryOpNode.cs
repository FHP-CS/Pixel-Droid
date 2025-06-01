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
        object x =  Left.Evaluate();
        object y = Right.Evaluate();
        switch (OperatorToken.Type)
        {
            case TokenType.AND:
               return x && y;
            case TokenType.Plus:
                return (int)x + (int)y;
            case TokenType.Minus:
                return (int)x - (int)y;
            case TokenType.Multiply:
                return (int)x * (int)y;
            case TokenType.Modulo:
                return (int)x % (int)y;
            case TokenType.Power:
                return (int)x^(int)y;
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