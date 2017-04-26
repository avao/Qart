using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestCasesPreprocessor
    {
        IEnumerable<TestCase> Execute(IEnumerable<TestCase> testCases, IDictionary<string, string> options);
    }
}
