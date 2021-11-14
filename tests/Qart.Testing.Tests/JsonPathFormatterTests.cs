using NUnit.Framework;
using Qart.Testing.Framework.Json;
using System.Collections.Generic;

namespace Qart.Testing.Tests
{
    public class JsonPathFormatterTests
    {
        [TestCase("$", "[0]", ExpectedResult = "$[0]")]
        [TestCase("$", "Prop", ExpectedResult = "$.Prop")]
        public string AddTokenTest(string current, string token)
        {
            return JsonPathFormatter.AddToken(current, token);
        }

        [TestCase]
        public void FormatConditionTest()
        {
            Execute(new[] { ("a", "'b'") }, "[?(@.a=='b')]");
            Execute(new[] { ("a", "3"), ("b", "'abc'") }, "[?(@.a==3 && @.b=='abc')]");
        }


        private static void Execute(IReadOnlyCollection<(string property, string value)> conditions, string expectedResult)
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
