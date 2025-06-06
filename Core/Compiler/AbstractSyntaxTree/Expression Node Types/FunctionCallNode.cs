//(for GetActualX(), IsBrushColor(), etc.)
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using PixelWallE.Common;
using PixelWallE.Execution;
public class FunctionCallNode : ExpressionNode
{
    public string FunctionName { get; }
    public Token FunctionNameToken { get; }

    public List<ExpressionNode> Arguments { get; }

    public FunctionCallNode(string functionName, Token functionNameToken, List<ExpressionNode> arguments)
    {
        FunctionName = functionName;
        FunctionNameToken = functionNameToken;
        Arguments = arguments;
    }
    public override string ToString()
    {
        string args = Arguments.Count > 0
            ? string.Join(", ", Arguments.Select(arg => arg.ToString()))
            : "";
        return $"{FunctionName}({args})";
    }
    public override object Evaluate(Interpreter interpreter)
    {
        List<object> evaluatedArgs = new List<object>();
        foreach (ExpressionNode argExpr in Arguments)
        {
            evaluatedArgs.Add(argExpr.Evaluate(interpreter));
        }
        // Delegate to the interpreter to handle the specific function call
        return interpreter.CallFunction(FunctionName, evaluatedArgs, FunctionNameToken);
    }
}