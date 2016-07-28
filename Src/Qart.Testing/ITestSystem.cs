using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public interface ITestSystem
    {
        TestCase GetTestCase(string id);
        IEnumerable<string> GetTestCaseIds();
    }
}
