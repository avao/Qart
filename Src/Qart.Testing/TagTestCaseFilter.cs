using Qart.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public class TagTestCaseFilter : ITestCaseFilter
    {
        private readonly ITagProvider _tagProvider;

        public TagTestCaseFilter(ITagProvider tagProvider)
        {
            _tagProvider = tagProvider;
        }

        public bool ShouldProcess(TestCase testCase, IDictionary<string, string> options)
        {
            IEnumerable<string> tags = _tagProvider.GetTags(testCase).Select(t => t.ToLower()).ToList();
           
            var includeTags = GetTagsAsHashSet(options, "includeTags");
            var excludeTags = GetTagsAsHashSet(options, "excludeTags");

            return !includeTags.Any()
                ? tags.All(t => !excludeTags.Contains(t))
                : tags.Any(t => includeTags.Contains(t) && !excludeTags.Contains(t));           
        }

        private HashSet<string> GetTagsAsHashSet(IDictionary<string, string> options, string tagName)
        {
            return new HashSet<string>(options.GetOptionalValue(tagName, Enumerable.Empty<string>(), t => t.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
