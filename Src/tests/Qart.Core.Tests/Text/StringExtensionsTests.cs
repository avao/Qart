using NUnit.Framework;
using Qart.Core.Text;

namespace Qart.Core.Tests.Text
{
    class StringExtensionsTests
    {
        [TestCase("a.bc", ".", ExpectedResult = "a")]
        [TestCase("abc.", ".", ExpectedResult = "abc")]
        [TestCase(".ab", ".", ExpectedResult = "")]
        [TestCase("ab<tag<tag>cd", "<tag>", ExpectedResult = "ab<tag")]
        public string LeftOfSucceeds(string value, string token)
        {
            return value.LeftOf(token);
        }

        [TestCase("a.bc", ".", ExpectedResult = "bc")]
        [TestCase("abc.", ".", ExpectedResult = "")]
        [TestCase(".ab", ".", ExpectedResult = "ab")]
        [TestCase("ab<tag<tag>cd", "<tag>", ExpectedResult = "cd")]
        public string RightOfSucceeds(string value, string token)
        {
            return value.RightOf(token);
        }

        [TestCase("abc.cd", ".", "abc", "cd")]
        [TestCase("abc.", ".", "abc", "")]
        [TestCase(".cd", ".", "", "cd")]
        [TestCase("<t>ab", "<t>", "", "ab")]
        [TestCase("ab<t>c", "<t>", "ab", "c")]
        [TestCase("abc<t>", "<t>", "abc", "")]
        [TestCase("a.bc.cd", ".", "a.bc", "cd")]
        [TestCase("abc..", ".", "abc.", "")]
        [TestCase(".c.d", ".", ".c", "d")]
        public void SplitOnLastSucceeds(string value, string token, string left, string right)
        {
            (var leftActual, var rightActual) = value.SplitOnLast(token);
            Assert.That(leftActual, Is.EqualTo(left));
            Assert.That(rightActual, Is.EqualTo(right));
        }

        [TestCase("abc.cd", ".", "abc", "cd")]
        [TestCase("abc.", ".", "abc", "")]
        [TestCase(".cd", ".", "", "cd")]
        [TestCase("<t>ab", "<t>", "", "ab")]
        [TestCase("ab<t>c", "<t>", "ab", "c")]
        [TestCase("abc<t>", "<t>", "abc", "")]
        [TestCase("a.bc.cd", ".", "a", "bc.cd")]
        [TestCase("abc..", ".", "abc", ".")]
        [TestCase(".c.d", ".", "", "c.d")]
        public void SplitOnFirstSucceeds(string value, string token, string left, string right)
        {
            (var leftActual, var rightActual) = value.SplitOnFirst(token);
            Assert.That(leftActual, Is.EqualTo(left));
            Assert.That(rightActual, Is.EqualTo(right));
        }

        [TestCase("ab{c}d", "{", "}", ExpectedResult = "c")]
        [TestCase("ab{}d", "{", "}", ExpectedResult = "")]
        [TestCase("{ab}cd", "{", "}", ExpectedResult = "ab")]
        [TestCase("a{bcd}{}", "{", "}", ExpectedResult = "bcd")]
        [TestCase("a<tag>bc</tag>", "<tag>", "</tag>", ExpectedResult = "bc")]
        [TestCase("a,,c", ",", ",", ExpectedResult = "")]
        [TestCase(",a,b", ",", ",", ExpectedResult = "a")]
        public string BetweenSucceeds(string value, string leftToken, string rightToken)
        {
            return value.Between(leftToken, rightToken);
        }

        [TestCase("ab.c..", ExpectedResult = "ab,c,,")]
        [TestCase("", ExpectedResult = "")]
        [TestCase("ab", ExpectedResult = "ab")]
        public string ToCsvSucceeds(string value)
        {
            return value.Split(".").ToCsv();
        }
    }
}
