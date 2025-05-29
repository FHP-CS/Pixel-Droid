// // Represents a numeric literal
// public class NumberNode : AstNode
// {
//     public int Value { get; }

//     public NumberNode(object value)
//     {
//         Value = int.Parse(value.ToString());
//     }
//      public override object Evaluate()
//     {
//         return Value; // The value of a NumberNode is just its number
//     }

//     public override string ToString() => Value.ToString();
// }