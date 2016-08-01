using Common.Logging;
using Qart.Testing.Framework;
using System.Xml.Linq;

namespace Qart.Testing
{
    public interface ITestCaseProcessor
    {
        void Process(TestCaseContext testCaseContext);
    }
}
