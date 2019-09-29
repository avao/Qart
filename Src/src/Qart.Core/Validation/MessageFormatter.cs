using Qart.Core.Text;
using System;
using System.Text;

namespace Qart.Core.Validation
{
    public static class MessageFormatter
    {
        public static string FormatMessageNotEqual(string actualContent, string expectedContent, int maxContextLength)
        {
            if (actualContent == null)
                return $"Actual content is null. Expected content: [{FormatForReporting(expectedContent, 0, maxContextLength)}]";

            if (expectedContent == null)
                return $"Expected content is null. Actual content: [{FormatForReporting(actualContent, 0, maxContextLength)}]";

            actualContent = ReplaceCharacters(actualContent);
            expectedContent = ReplaceCharacters(expectedContent);

            int diffIndex = 0;
            var minLen = Math.Min(actualContent.Length, expectedContent.Length);
            while (diffIndex < minLen && actualContent[diffIndex] == expectedContent[diffIndex])
            {
                ++diffIndex;
            }

            int halfContextLength = maxContextLength / 2;
            int startIndex = diffIndex > halfContextLength ? diffIndex - halfContextLength : 0;

            var actualToReport = FormatForReporting(actualContent, startIndex, maxContextLength);
            var expectedToReport = FormatForReporting(expectedContent, startIndex, maxContextLength);
            var diffPositionToReport = ReportDiffPosition(startIndex, diffIndex);
            return $"Expected string length {expectedContent.Length} but was {actualContent.Length}. Content differ at index {diffIndex}.\n Expected: [{expectedToReport}]\n But was:  [{actualToReport}]\n -----------{diffPositionToReport}";
        }

        private static string ReplaceCharacters(string content)
        {
            return content.Replace("\r", "").Replace("\n", "\\n"); //TODO other characters like \t
        }

        private static string FormatForReporting(string content, int startIndex, int maxLength)
        {
            string prefix = startIndex == 0 ? string.Empty : "...";
            string suffix = content.Length > maxLength + startIndex ? "..." : string.Empty;
            return $"{prefix}{content.SubstringUpTo(startIndex, maxLength)}{suffix}";
        }
        private static string ReportDiffPosition(int startIndex, int diffIndex)
        {
            if (startIndex > 0)
                diffIndex += 3;//"..."
            return new string('-', diffIndex - startIndex) + "^";
        }
    }
}
