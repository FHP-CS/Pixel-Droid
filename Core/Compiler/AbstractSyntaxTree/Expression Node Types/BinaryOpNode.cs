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
        object leftValue = Left.Evaluate(Interpreter);
        object rightValue = Right.Evaluate(Interpreter);
        switch (OperatorToken.Type)
        {
            //Arithmetics
            case TokenType.Plus:
                if (leftValue is int LplusInt && rightValue is int RplusInt)
                    return LplusInt + RplusInt;
                throw new RuntimeError($"Operands for '+' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);
            case TokenType.Minus:
                if (leftValue is int LminusInt && rightValue is int RminusInt)
                    return LminusInt - RminusInt;
                throw new RuntimeError($"Operands for '-' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);
            case TokenType.Multiply:
                if (leftValue is int LmultInt && rightValue is int RmultInt)
                    return LmultInt * RmultInt;
                throw new RuntimeError($"Operands for '*' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);
            case TokenType.Modulo:
                if (leftValue is int lIntMod && rightValue is int rIntMod)
                {
                    if (rIntMod == 0)   throw new RuntimeError("Modulo by zero.", OperatorToken);
                    return lIntMod % rIntMod;
                }
                throw new RuntimeError($"Operands for '%' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);
            case TokenType.Power:
                if (leftValue is int LpowInt && rightValue is int RpowInt)
                {
                    if (RpowInt < 0) // Math.Pow with negative exponent returns double, we need int result.
                        throw new RuntimeError("Negative exponent in integer power operation (**) is supported for integer results only.", OperatorToken);
                    return LpowInt ^ RpowInt;
                }
                throw new RuntimeError($"Operands for '**' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);
            case TokenType.Divide:
                if (leftValue is int LdivInt && rightValue is int RdivInt)
                {
                    if (LdivInt == 0) throw new DivideByZeroException("Error: Division by zero.");
                    return LdivInt / RdivInt;
                }
                throw new RuntimeError($"Operands for '/' must be integers. Got {leftValue?.GetType().Name} and {rightValue?.GetType().Name}.", OperatorToken);



            // --- Equality Operations ---
            case TokenType.Equal_Equal: // ==
                return AreEqual(leftValue, rightValue); // Helper for flexible equality
                                                        // --- Comparison Operations ---
                                                        // For comparisons, we expect numbers primarily.
                                                        // The PDF implies comparison results are used in GoTo (condition), so they should be boolean.
            case TokenType.Greater_Than: // >
                EnsureNumericOperands(leftValue, rightValue, OperatorToken);
                return Convert.ToInt32(leftValue) > Convert.ToInt32(rightValue);
            case TokenType.Less_Than: // <
                EnsureNumericOperands(leftValue, rightValue, OperatorToken);
                return Convert.ToInt32(leftValue) < Convert.ToInt32(rightValue);
            case TokenType.Greater_Equal: // >=
                EnsureNumericOperands(leftValue, rightValue, OperatorToken);
                return Convert.ToInt32(leftValue) >= Convert.ToInt32(rightValue);
            case TokenType.Less_Equal: // <=
                EnsureNumericOperands(leftValue, rightValue, OperatorToken);
                return Convert.ToInt32(leftValue) <= Convert.ToInt32(rightValue);

            // --- Logical Operations ---
            // Operands should evaluate to booleans (which might be represented as 0 or 1 from comparisons)
            case TokenType.AND: // &&
                // Short-circuiting AND: if left is false, result is false, don't eval right.
                // However, our current structure evaluates both left and right before this switch.
                // To implement true short-circuiting, the Interpreter logic or the node structure
                // for AND/OR would need to be different (e.g., evaluate left, then conditionally evaluate right).
                // For now, standard evaluation:
                EnsureBooleanOperands(leftValue, rightValue, OperatorToken, Interpreter); // Use interpreter to get truthiness
                return IsTruthy(leftValue, Interpreter) && IsTruthy(rightValue, Interpreter);
            case TokenType.OR: // ||
                EnsureBooleanOperands(leftValue, rightValue, OperatorToken, Interpreter); // Use interpreter to get truthiness
                return IsTruthy(leftValue, Interpreter) || IsTruthy(rightValue, Interpreter);

            default:
                throw new RuntimeError($"Unsupported binary operator '{OperatorToken.Lexeme}'.", OperatorToken);

        }
    }
    public override string ToString() => $"({Left} {OperatorToken.Literal} {Right})";
    // Helper for type checking numeric operations
    private void EnsureNumericOperands(object left, object right, Token opToken)
    {
        if (!(left is int) || !(right is int))
        {
            throw new RuntimeError($"Operands for '{opToken.Lexeme}' must both be integers. Got {left?.GetType().Name} and {right?.GetType().Name}.", opToken);
        }
    }
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