//(for var <- Expression)

// var <- expression
public class AssignmentNode : StatementNode
{
    public string VariableName { get; }
    public ExpressionNode ValueExpression { get; }

    public AssignmentNode(string variableName, ExpressionNode valueExpression)
    {
        VariableName = variableName;
        ValueExpression = valueExpression;
    }
    public override string ToString() => $"{VariableName} <- {ValueExpression}";
}