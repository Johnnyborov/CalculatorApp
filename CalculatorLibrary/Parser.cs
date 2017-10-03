using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CalculatorLibrary
{
    internal class Parser
    {
        /// <summary>
        /// Parses the given expression and returns the result in Reverse Polish Notation form if expression is valid or throws an exception if it isn't.
        /// </summary>
        ///<param name="expression">Expression to parse.</param>
        internal static Stack<ExpressionElement> ParseExpression(string expression)
        {

            // Clear white space
            string pattern = @"(\s+)";
            Regex regex = new Regex(pattern);
            expression = regex.Replace(expression, "");

            // Validate the expression
            pattern = @"^[\d|\.|\+|\-|\*|\/|\^|\(|\)]*$";
            regex = new Regex(pattern);
            if (expression.Length == 0 || !regex.IsMatch(expression))
            {
                throw new Exception("Not a valid expression");
            }

            var stack = new Stack<ExpressionElement>();
            var helpStack = new Stack<ExpressionElement>();
            ExpressionElement currElem, tempResult, peekResult;
            ExpressionElement prevElem = null;

            helpStack.Push(null);

            while (expression.Length > 0)
            {
                currElem = GetFirstExpressionElement(expression);

                if (currElem.Type == ElementTypes.Number)
                {
                    if (prevElem == null ||
                        prevElem.Type == ElementTypes.Operation ||
                        prevElem.Type == ElementTypes.Bracket && (prevElem as Bracket).BracketType == BracketTypes.Opening)
                    {
                        stack.Push(currElem);
                    }
                    else
                    {
                        throw new Exception("Not a valid expression");
                    }
                }

                else if (currElem.Type == ElementTypes.Bracket)
                {
                    if ((currElem as Bracket).BracketType == BracketTypes.Opening)
                    {
                        if (prevElem == null ||
                            prevElem.Type == ElementTypes.Operation ||
                            prevElem.Type == ElementTypes.Bracket && (prevElem as Bracket).BracketType == BracketTypes.Opening)
                        {
                            helpStack.Push(currElem);
                        }
                        else
                        {
                            throw new Exception("Not a valid expression");
                        }
                    }
                    else if ((currElem as Bracket).BracketType == BracketTypes.Closing)
                    {
                        if (prevElem?.Type == ElementTypes.Number ||
                            prevElem?.Type == ElementTypes.Bracket && (prevElem as Bracket).BracketType == BracketTypes.Closing)
                        {
                            peekResult = helpStack.Peek();
                            while (peekResult?.Type == ElementTypes.Operation)
                            {
                                tempResult = helpStack.Pop();
                                stack.Push(tempResult);
                                peekResult = helpStack.Peek();
                            }
                            if ((peekResult as Bracket).BracketType == BracketTypes.Opening)
                            {
                                helpStack.Pop();
                            }
                        }
                        else
                        {
                            throw new Exception("Not a valid expression");
                        }                      
                    }
                }

                else if (currElem.Type == ElementTypes.Operation)
                {
                    if ((currElem as Operation).Op == OperationTypes.Minus)
                    {
                        if (prevElem == null || prevElem.Type == ElementTypes.Bracket && (prevElem as Bracket).BracketType == BracketTypes.Opening)
                        {
                            prevElem = new Number
                            {
                                Type = ElementTypes.Number,
                                StringLength = 1,
                                Value = 0
                            };

                            stack.Push(prevElem);
                        }
                    }

                    if (prevElem.Type == ElementTypes.Number ||
                        prevElem.Type == ElementTypes.Bracket && (prevElem as Bracket).BracketType == BracketTypes.Closing)
                    {
                        peekResult = helpStack.Peek();
                        while ( peekResult?.Type != ElementTypes.Bracket &&
                                (currElem as Operation).Priority <= (peekResult as Operation)?.Priority &&
                                (currElem as Operation).Priority != OperationPriorities.Power)
                        {
                            tempResult = helpStack.Pop();
                            stack.Push(tempResult);
                            peekResult = helpStack.Peek();
                        }
                        helpStack.Push(currElem);
                    }
                    else
                    {
                        throw new Exception("Not a valid expression");
                    }                 
                }

                prevElem = currElem;
                expression = expression.Remove(0, currElem.StringLength);
            }

            while (helpStack.Peek() != null)
            {
                tempResult = helpStack.Pop();
                if (!(tempResult.Type == ElementTypes.Bracket && (tempResult as Bracket).BracketType == BracketTypes.Opening))
                {
                    stack.Push(tempResult);
                }
            }

            return stack;
        }

        /// <summary>
        /// Tries to parse the expression into Reverse Polish Notation form. If successful returns true, otherwise returns false.
        /// </summary>
        ///<param name="expression">Expression to parse.</param>
        ///<param name="stack">Stack to keep the result in Reverse Polish Notation form</param>
        internal static bool TryParseExpression(string expression, Stack<ExpressionElement> stack)
        {
            return true;
        }

        private static ExpressionElement GetFirstExpressionElement(string expression)
        {
            switch (expression[0])
            {
                case '+':
                    return new Operation {
                        Type = ElementTypes.Operation,
                        StringLength = 1,
                        Op = OperationTypes.Plus,
                        Priority = OperationPriorities.PlusMinus
                    };
                case '-':
                    return new Operation
                    {
                        Type = ElementTypes.Operation,
                        StringLength = 1,
                        Op = OperationTypes.Minus,
                        Priority = OperationPriorities.PlusMinus
                    };
                case '*':
                    return new Operation
                    {
                        Type = ElementTypes.Operation,
                        StringLength = 1,
                        Op = OperationTypes.Multiply,
                        Priority = OperationPriorities.MultiplyDivide
                    };
                case '/':
                    return new Operation
                    {
                        Type = ElementTypes.Operation,
                        StringLength = 1,
                        Op = OperationTypes.Divide,
                        Priority = OperationPriorities.MultiplyDivide
                    };
                case '^':
                    return new Operation
                    {
                        Type = ElementTypes.Operation,
                        StringLength = 1,
                        Op = OperationTypes.Power,
                        Priority = OperationPriorities.Power
                    };
                case '(':
                    return new Bracket
                    {
                        Type = ElementTypes.Bracket,
                        StringLength = 1,
                        BracketType = BracketTypes.Opening
                    };
                case ')':
                    return new Bracket
                    {
                        Type = ElementTypes.Bracket,
                        StringLength = 1,
                        BracketType = BracketTypes.Closing
                    };
                default:
                    string numberAsString = ReturnFirstNumber(expression);

                    double value = Double.Parse(numberAsString, CultureInfo.InvariantCulture);

                    return new Number
                    {
                        Type = ElementTypes.Number,
                        StringLength = numberAsString.Length,
                        Value = value
                    };
            }

        }

        private static string ReturnFirstNumber(string expression)
        {
            string pattern = @"^(\d+)\.(\d+)|^(\d+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(expression);
            return match.Value;
        }
    }
}
