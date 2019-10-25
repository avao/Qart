using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestStorage
    {
        TestCase GetTestCase(string id);
        IEnumerable<string> GetTestCaseIds();
    }
}
