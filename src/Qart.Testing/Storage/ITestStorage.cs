using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Testing.Storage
{
    public interface ITestStorage
    {
        TestCase GetTestCase(string id);
        IReadOnlyCollection<string> GetTestCaseIds();
    }
}
