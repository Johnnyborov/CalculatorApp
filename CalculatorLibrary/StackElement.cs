using System.Globalization;

namespace CalculatorLibrary
{
    internal enum ElementTypes
    {
        Number,
        Operation,
        Bracket
    }

    internal enum OperationTypes
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Power
    }

    internal enum OperationPriorities
    {
        PlusMinus = 1,
        MultiplyDivide = 2,
        Power = 3
    }

    internal enum BracketTypes
    {
        Opening,
        Closing
    }

    internal class ExpressionElement
    {
        internal ElementTypes Type { get; set; }

        internal int StringLength { get; set; }
    }

    internal class Number : ExpressionElement
    {
        internal double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal class Bracket : ExpressionElement
    {
        internal BracketTypes BracketType { get; set; }

        public override string ToString()
        {
            switch (BracketType)
            {
                case BracketTypes.Opening:
                    return "(";
                case BracketTypes.Closing:
                    return ")";
                default:
                    return "BracketBracketTypeNotSet";
            }
        }
    }

    internal class Operation : ExpressionElement
    {
        internal OperationTypes Op { get; set; }

        internal OperationPriorities Priority { get; set; }

        public override string ToString()
        {
            switch (Op)
            {
                case OperationTypes.Plus:
                    return "+";
                case OperationTypes.Minus:
                    return "-";
                case OperationTypes.Multiply:
                    return "*";
                case OperationTypes.Divide:
                    return "/";
                case OperationTypes.Power:
                    return "^";
                default:
                    return "OperationValueNotSet";
            }
        }
    }
}
