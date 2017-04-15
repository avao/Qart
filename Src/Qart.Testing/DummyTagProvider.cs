using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public class DummyTagProvider : ITagProvider
    {
        public IEnumerable<string> GetTags(TestCase testCase)
        {
            return Enumerable.Empty<string>();
        }
    }
}
