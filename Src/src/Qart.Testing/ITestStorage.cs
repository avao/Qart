using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public interface ITestStorage
    {
        TestCase GetTestCase(string id);
        IEnumerable<string> GetTestCaseIds();
    }
}
