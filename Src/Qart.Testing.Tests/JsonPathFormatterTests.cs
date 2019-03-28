using NUnit.Framework;
using Qart.Testing.Framework.Json;
using System.Collections.Generic;

namespace Qart.Testing.Tests
{
    [TestFixture]
    class JsonPathFormatterTests
    {
        [TestCase("$", "[0]", ExpectedResult = "$[0]")]
        [TestCase("$", "Prop", ExpectedResult = "$.Prop")]
        public string AddTokenTest(string current, string token)
        {
            return JsonPathFormatter.AddToken(current, token);
        }

        private static IEnumerable<object[]> FormatConditionTestSource()
        {
            yield return new object[] { new[] { ("a", "'b'") }, "[?(@.a=='b')]" };
            yield return new object[] { new[] { ("a", "3"), ("b", "'abc'") }, "[?(@.a==3 && @.b=='abc')]" };
        }

        [TestCaseSource("FormatConditionTestSource")]
        public void FormatConditionTest(IEnumerable<(string property, string value)> conditions, string expectedResult)
        {
            Assert.That(JsonPathFormatter.FormatAndCondition(conditions), Is.EqualTo(expectedResult));
        }

        [TestCase(0, ExpectedResult = "[0]")]
        public string FormatIndexTest(int index)
        {
            return JsonPathFormatter.FormatIndex(index);
        }
    }
}
