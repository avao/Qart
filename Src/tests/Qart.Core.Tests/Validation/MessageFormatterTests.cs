using NUnit.Framework;
using Qart.Core.Validation;

namespace Qart.Core.Tests.Validation
{
    public class MessageFormatterTests
    {
        [TestCase("abc", null, 50, ExpectedResult = "Expected content is null. Actual content: [abc]")]
        [TestCase(null, "xyz", 50, ExpectedResult = "Actual content is null. Expected content: [xyz]")]
        [TestCase(null, "xyz", 2, ExpectedResult = "Actual content is null. Expected content: [xy...]")]
        [TestCase("This is a long string that", "This is a long string that adds value", 100, ExpectedResult = "Expected string length 37 but was 26. Content differ at index 26.\n Expected: [This is a long string that adds value]\n But was:  [This is a long string that]\n -------------------------------------^")]
        [TestCase("This is a long string that's adds value", "This is a long string that adds value", 10, ExpectedResult = "Expected string length 37 but was 39. Content differ at index 26.\n Expected: [... that adds...]\n But was:  [... that's ad...]\n -------------------^")]
        public string FormatNotEqual(string actual, string expected, int maxContext)
        {
            var result = MessageFormatter.FormatMessageNotEqual(actual, expected, maxContext);
            return result;
        }
    }
}
