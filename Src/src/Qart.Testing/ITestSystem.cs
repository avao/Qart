using System.Collections;
using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestSystem
    {
        IEnumerable<TestCase> GetTestCases(IDictionary<string, string> options);
    }
}
