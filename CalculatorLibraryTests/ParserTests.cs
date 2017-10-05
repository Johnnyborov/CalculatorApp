using NUnit.Framework;
using System;
using System.Text;
using CalculatorLibrary;


namespace CalculatorLibraryTests
{
    [TestFixture]
    class ParserTests
    {
        // basic
        [TestCase("4", "4")]
        [TestCase("3+4", "3 4 +")]
        [TestCase("2+3*4*5-9", "2 3 4 * 5 * + 9 -")]
        // brackets
        [TestCase("(3+4)", "3 4 +")]
        [TestCase("-3+4", "0 3 - 4 +")]      
        [TestCase("(-3+4)", "0 3 - 4 +")]
        [TestCase("(3)+(-4)", "3 0 4 - +")]
        [TestCase("(2+3)*4*(5-9)^5", "2 3 + 4 * 5 9 - 5 ^ *")]
        // special power priority
        [TestCase("2+3^2^4", "2 3 2 4 ^ ^ +")]
        // not an integer
        [TestCase("324.02^0.5+345*44.55", "324.02 0.5 ^ 345 44.55 * +")]
        // spaces
        [TestCase("   2 +  3   * 4*  5- 9 ", "2 3 4 * 5 * + 9 -")]
        public void ParseExpression_CorrectInput_CorrectResultReturned(string input, string expected)
        {
            var stack = Parser.ParseExpression(input);

            var sb = new StringBuilder("");          
            foreach (var element in stack)
            {
                sb.Insert(0, $"{element.ToString()} ");
            }
            string result = sb.ToString();
            result = result.TrimEnd();
            Assert.AreEqual(expected, result);
        }

        // empty string
        [TestCase("")]
        // starts with wrong symbol
        [TestCase("*4+2")]
        [TestCase(")2+2")]
        // ends with wrong symbol
        [TestCase("2+4-")]
        [TestCase("2+2(")]
        // bracket imbalance of amount
        [TestCase("3+(2+2))")]
        [TestCase("(3+(2+2)")]
        // bracket imbalance of order
        [TestCase("2)*(3+2")]
        // invalid previous symbol before number
        [TestCase("2+(4)2")]
        // invalid previous symbol before operation
        [TestCase("2++2")]
        [TestCase("2)(+2")]
        // invalid previous symbol before opening bracket
        [TestCase("2+2(4)")]
        [TestCase("2+(3)(4)")]
        // invalid previous symbol before closing bracket
        [TestCase("2+(2*)-(4)")]
        [TestCase("2+()-(4)")]
        // invalid symbols
        [TestCase("23=47")]
        // dots in the wrong place
        [TestCase(".2")]
        [TestCase("2.")]
        [TestCase("2..4")]
        [TestCase("24.+2")]
        [TestCase("24+.2")]
        public void ParseExpression_InvalidInput_Throw(string input)
        {
            var ex = Assert.Catch<Exception>(() => Parser.ParseExpression(input));

            StringAssert.Contains("Not a valid expression", ex.Message);
        }
    }
}
