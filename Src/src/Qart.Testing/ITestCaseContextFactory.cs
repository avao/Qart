using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestCaseContextFactory
    {
        TestCaseContext CreateContext(TestCase testCase, IDictionary<string, string> options);
        void Release(TestCaseContext context);
    }
}
