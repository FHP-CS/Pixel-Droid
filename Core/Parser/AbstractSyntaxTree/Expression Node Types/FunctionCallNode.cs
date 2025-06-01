//(for GetActualX(), IsBrushColor(), etc.)
using System;
using System.Collections.Generic;
using System.Collections;
public class FunctionCallNode : ExpressionNode
{
    public string FunctionName {get;}
    public List<ExpressionNode> Arguments {get;}

    public FunctionCallNode(string functionName, List<ExpressionNode> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }
    public override string ToString()
    {
        string args = Arguments.Count > 0
            ? string.Join(", ", Arguments.Select(arg => arg.ToString()))
            : "";
        return $"{FunctionName}({args})";
    }
}