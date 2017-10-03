using NUnit.Framework;
using System;
using CalculatorLibrary;


namespace CalculatorLibraryTests
{
    [TestFixture]
    class CalculatorTests
    {
        // basic
        [TestCase("4", 4)]          // 4
        [TestCase("3+4", 7)]        // 3 4 +
        [TestCase("2+3*4*5-9", 53)] // 2 3 4 * 5 * + 9 -
        [TestCase("3+4*2^3", 35)]   // 2 4 2 3 ^ * +
        // not an integer
        [TestCase("324.02^0.5+345*44.55", 15387.7505555)] // 324.02 0.5 ^ 345 44.55 * +
        // division by zero
        [TestCase("4+2/(-3+3)", Double.PositiveInfinity)]   // 2 4 2 3 ^ * +
        [TestCase("4-2/(-3+3)", Double.NegativeInfinity)]   // 2 4 2 3 ^ * -
        public void CalculateExpression_CorrectInput_CorrectResultReturned(string input, double expected)
        {
            double result = Calculator.CalculateExpression(input);

            Assert.AreEqual(expected, result, 0.001);
        }

        // invalid string
        [TestCase("2+()")]
        public void CalculateExpression_InvalidInput_Throw(string input)
        {
            var ex = Assert.Catch<Exception>(() => Calculator.CalculateExpression(input));

            StringAssert.Contains("Not a valid expression", ex.Message);
        }
    }
}
