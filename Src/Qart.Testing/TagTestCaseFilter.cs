using Qart.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public class TagTestCaseFilter : ITestCaseFilter
    {
        private readonly ITagProvider _tagProvider;
        private readonly IDictionary<string, string> _options;

        public TagTestCaseFilter(ITagProvider tagProvider, IDictionary<string, string> options)
        {
            _tagProvider = tagProvider;
            _options = options;
        }

        public bool ShouldProcess(TestCase testCase)
        {
            IEnumerable<string> tags = _tagProvider.GetTags(testCase).Select(t => t.ToLower()).ToList();
           
            var includeTags = GetTagsAsHashSet("includeTags");
            var excludeTags = GetTagsAsHashSet("excludeTags");

            return !includeTags.Any()
                ? tags.All(t => !excludeTags.Contains(t))
                : tags.Any(t => includeTags.Contains(t) && !excludeTags.Contains(t));           
        }

        private HashSet<string> GetTagsAsHashSet(string tagName)
        {
            return new HashSet<string>(_options.GetOptionalValue(tagName, Enumerable.Empty<string>(), t => t.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
