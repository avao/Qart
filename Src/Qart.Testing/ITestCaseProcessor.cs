using Common.Logging;
using System.Xml.Linq;

namespace Qart.Testing
{
    public interface ITestCaseProcessor //TODO separate assembly
    {
        void Process(TestSession testSession, TestCase testCase, ILog logger);
        XDocument GetDescription(TestCase testCase);
    }
}
