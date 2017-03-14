using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITagProvider
    {
        IEnumerable<string> GetTags(TestCase testCase);
    }
}
