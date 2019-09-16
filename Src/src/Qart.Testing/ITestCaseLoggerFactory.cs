using Qart.Testing.Framework.Logging;

namespace Qart.Testing
{
    public interface ITestCaseLoggerFactory
    {
        IDisposableLogger GetLogger(TestCase testCase);
    }
}
