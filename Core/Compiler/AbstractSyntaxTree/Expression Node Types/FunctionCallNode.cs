//(for GetActualX(), IsBrushColor(), etc.)
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using PixelWallE.Common;
using PixelWallE.Execution;
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
    public override object Evaluate(Interpreter interpreter)
    {
        string name = FunctionName.ToLower();
        if( name == "getactualx")     return interpreter.WallEInstance.X;
        if( name == "getactualy")     return interpreter.WallEInstance.Y;
        if( name == "getcanvassize")     return interpreter.Canvas.Height;

        throw new NotImplementedException();
    }
}