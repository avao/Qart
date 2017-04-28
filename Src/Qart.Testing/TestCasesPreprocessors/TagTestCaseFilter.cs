using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.TestCasesPreprocessors
{
    public class TagTestCaseFilter : ITestCasesPreprocessor
    {
        private readonly ITagProvider _tagProvider;

        public TagTestCaseFilter(ITagProvider tagProvider = null)
        {
            _tagProvider = tagProvider;
        }

        public IEnumerable<TestCase> Execute(IEnumerable<TestCase> testCases, IDictionary<string, string> options)
        {
            if (_tagProvider != null)
                testCases = testCases.Where(_ => ShouldProcess(_, options));
            return testCases;
        }

        private bool ShouldProcess(TestCase testCase, IDictionary<string, string> options)
        {
            IEnumerable<string> tags = _tagProvider.GetTags(testCase).Select(t => t.ToLower()).ToList();

            var includeTags = new HashSet<string>(options.GetIncludeTags(), StringComparer.InvariantCultureIgnoreCase);
            var excludeTags = new HashSet<string>(options.GetExcludeTags(), StringComparer.InvariantCultureIgnoreCase);

            return !includeTags.Any()
                ? tags.All(t => !excludeTags.Contains(t))
                : tags.Any(t => includeTags.Contains(t) && !excludeTags.Contains(t));
        }
    }
}
