using Qart.Core.Collections;
using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Framework
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

            var includeTags = new HashSet<string>(options.GetIncludeTags(), StringComparer.InvariantCultureIgnoreCase);
            var excludeTags = new HashSet<string>(options.GetExcludeTags(), StringComparer.InvariantCultureIgnoreCase);

            return !includeTags.Any()
                ? tags.All(t => !excludeTags.Contains(t))
                : tags.Any(t => includeTags.Contains(t) && !excludeTags.Contains(t));           
        }



    }
}
