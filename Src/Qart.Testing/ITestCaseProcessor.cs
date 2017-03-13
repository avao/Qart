using Qart.Testing.Framework;

namespace Qart.Testing
{
    public interface ITestCaseProcessor
    {
        void Process(TestCaseContext testCaseContext);
    }
}
