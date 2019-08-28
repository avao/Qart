using Qart.Core.Logging;

namespace Qart.Testing
{
    public interface ITestCaseLoggerFactory
    {
        IDisposableLogger GetLogger(TestCase testCase);
    }
}
