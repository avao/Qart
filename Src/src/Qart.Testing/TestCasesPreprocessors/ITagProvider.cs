using System.Collections.Generic;

namespace Qart.Testing.TestCasesPreprocessors
{
    public interface ITagProvider
    {
        IEnumerable<string> GetTags(TestCase testCase);
    }
}
