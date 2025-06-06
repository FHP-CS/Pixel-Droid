using System;
using PixelWallE.Common;
using PixelWallE.Execution;
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
    public override object Evaluate(Interpreter Interpreter)
    {
        object leftValueObj = Left.Evaluate(Interpreter);
        object rightValueObj = Right.Evaluate(Interpreter);
        if (OperatorToken.Type == TokenType.Plus ||
             OperatorToken.Type == TokenType.Minus ||
             OperatorToken.Type == TokenType.Multiply ||
             OperatorToken.Type == TokenType.Divide ||
             OperatorToken.Type == TokenType.Modulo ||
             OperatorToken.Type == TokenType.Power)
            {
            if (!(leftValueObj is int) || !(rightValueObj is int))
            {
                // Type error: could also try to convert, but PDF implies integers.
                // For example, Convert.ToInt32 might work but could throw if not convertible.
                // Let's be strict for now as per "Número entero".
                throw new RuntimeError($"Operands for arithmetic operator '{OperatorToken.Lexeme}' must be integers.", OperatorToken);
            }
            int leftInt = (int)leftValueObj;
            int rightInt = (int)rightValueObj;

            switch (OperatorToken.Type)
            {
                // --- Arithmetics Operations ---
                case TokenType.Plus:
                    return leftInt + rightInt;
                case TokenType.Minus:
                    return leftInt - rightInt;
                case TokenType.Multiply:
                    return leftInt * rightInt;
                case TokenType.Modulo:
                    if (rightInt == 0) throw new RuntimeError("Modulo by zero.", OperatorToken);
                    return leftInt % rightInt;
                case TokenType.Power:
                    if (rightInt < 0) throw new RuntimeError("Negative exponent in integer power operation (**) is supported for integer results only.", OperatorToken);
                    return (int)Math.Pow(leftInt, rightInt);
                case TokenType.Divide:
                    if (rightInt == 0) throw new DivideByZeroException("Error: Division by zero.");
                    return leftInt / rightInt;
            }
        }
        // --- Comparison Operations ---
        if (OperatorToken.Type == TokenType.Greater_Than ||
            OperatorToken.Type == TokenType.Greater_Equal ||
            OperatorToken.Type == TokenType.Less_Than ||
            OperatorToken.Type == TokenType.Less_Equal ||
            OperatorToken.Type == TokenType.Equal_Equal)
        {
            if (OperatorToken.Type == TokenType.Equal_Equal) return AreEqual(leftValueObj, rightValueObj); // Helper for flexible equality
            if (leftValueObj is int lInt && rightValueObj is int rInt)
            {
                switch (OperatorToken.Type)
                {                                                   // The PDF implies comparison results are used in GoTo (condition), so they should be boolean.
                    case TokenType.Greater_Than: return lInt > rInt;
                    case TokenType.Less_Than: return lInt < rInt;
                    case TokenType.Greater_Equal: return lInt >= rInt;
                    case TokenType.Less_Equal: return lInt <= rInt;
                }
            }
        }
        // --- Logical Operations ---
        // Operands must be booleans.
        if (OperatorToken.Type == TokenType.AND ||
            OperatorToken.Type == TokenType.OR)
        {
            EnsureBooleanOperands(leftValueObj, rightValueObj, OperatorToken, Interpreter); // Use interpreter to get truthiness
            switch (OperatorToken.Type)
            {
                case TokenType.AND: // &&
                    return IsTruthy(leftValueObj, Interpreter) && IsTruthy(rightValueObj, Interpreter);
                case TokenType.OR: // ||
                    return IsTruthy(leftValueObj, Interpreter) || IsTruthy(rightValueObj, Interpreter);

            }
        }
        throw new RuntimeError($"Unsupported binary operator '{OperatorToken.Lexeme}'.",OperatorToken);
    }
    public override string ToString() => $"({Left} {OperatorToken.Literal} {Right})";
    // Helper for type checking numeric operations
    private bool AreEqual(object left, object right)
    {
        if (left == null && right == null) return true;// son null ambos
        if (left == null || right == null) return false;//solo 1 es null
        if (left.GetType() == right.GetType())
        {
            return left.Equals(right);
        }
        //Comparison 0/1 with true/false, with boolean
        if (left is int lInt && right is bool rBool)
            return (lInt != 0) == rBool;
        if (right is int rInt && left is bool lBool)//assossiative
            return (rInt != 0) == lBool;
        return false;  //Different types not covered by specific rules are not equal

    }
    private void EnsureBooleanOperands(object left, object right, Token opToken, Interpreter Interpreter)
    {
        if (!IsConsideredBoolean(left, Interpreter) || !IsConsideredBoolean(right, Interpreter))
            throw new RuntimeError($"Operands for '{opToken.Lexeme}' must be boolean-like (true/false or 0/1). Got {left?.GetType().Name} and {right?.GetType().Name}.", opToken);
    }
    private bool IsConsideredBoolean(object value, Interpreter interpreter)
    {
        if (value is bool) return true;
        if (value is int valInt && (valInt == 0 || valInt == 1)) return true; // As per PDF example for GoTo
        // Potentially other types if your language defines their truthiness
        return false;
    }
    private bool IsTruthy(object value, Interpreter interpreter)
    {
        if (value == null) return false; // Null is generally falsey
        if (value is bool b) return b;
        if (value is int i) return i != 0;
        return true; // Default to true for other non-null types if not specified otherwise
    }

}