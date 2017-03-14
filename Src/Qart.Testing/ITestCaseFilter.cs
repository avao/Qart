using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestCaseFilter
    {
        bool ShouldProcess(TestCase testCase, IDictionary<string, string> options);
    }
}
