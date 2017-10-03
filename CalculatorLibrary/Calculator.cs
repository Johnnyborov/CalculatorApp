using System;
using System.Collections.Generic;

namespace CalculatorLibrary
{
    public class Calculator
    {
        /// <summary>
        /// Calculates the result of the given expression if expression is valid or throws an exception if it isn't.
        /// </summary>
        ///<param name="expression">Expression to calculate.</param>
        public static double CalculateExpression(string expression)
        {
            var stack = Parser.ParseExpression(expression);
            var helpStack = new Stack<ExpressionElement>();

            ExpressionElement currElem, lastOp, tempResult;

            helpStack.Push(null);

            do
            {
                currElem = stack.Pop();

                if (currElem.Type == ElementTypes.Number)
                {
                    if (stack.Count == 0)
                    {
                        return (currElem as Number).Value;
                    }
                    else if (stack.Peek().Type == ElementTypes.Operation)
                    {
                        helpStack.Push(currElem);
                    }
                    else if (stack.Peek().Type == ElementTypes.Number)
                    {
                        tempResult = stack.Pop();
                        lastOp = helpStack.Pop();
                        double value;
                        switch ((lastOp as Operation).Op)
                        {
                            case OperationTypes.Plus:
                                value = (tempResult as Number).Value + (currElem as Number).Value;
                                break;
                            case OperationTypes.Minus:
                                value = (tempResult as Number).Value - (currElem as Number).Value;
                                break;
                            case OperationTypes.Multiply:
                                value = (tempResult as Number).Value * (currElem as Number).Value;
                                break;
                            case OperationTypes.Divide:
                                value = (tempResult as Number).Value / (currElem as Number).Value;
                                break;
                            case OperationTypes.Power:
                                value = Math.Pow((tempResult as Number).Value, (currElem as Number).Value);
                                break;
                            default:
                                throw new Exception("Algorithm mistake 2, this part of code shouldn't be ever reached");
                        }
                        tempResult = new Number
                        {
                            Type = ElementTypes.Number,
                            Value = value
                        };
                        stack.Push(tempResult);
                        while (helpStack.Peek()?.Type == ElementTypes.Number)
                        {
                            tempResult = helpStack.Pop();
                            stack.Push(tempResult);
                        }
                    }
                }
                else if (currElem.Type == ElementTypes.Operation)
                {
                    helpStack.Push(currElem);
                }
            } while (true);

            throw new Exception("Algorithm mistake 1, this part of code shouldn't be ever reached");
        }

        /// <summary>
        /// Calculates the result of the given expression. If successful returns true, otherwise returns false.
        /// </summary>
        ///<param name="expression">Expression to calculate</param>
        ///<param name="result">The result of calculation if it was successful.</param>
        ///<example>lol</example>
        public static bool TryCalculateExpression(string expression, out double result)
        {
            result = 0;
            return true;
        }


    }
}
