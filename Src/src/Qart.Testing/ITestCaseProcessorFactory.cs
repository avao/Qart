namespace Qart.Testing
{
    public interface ITestCaseProcessorFactory
    {
        ITestCaseProcessor GetProcessor(TestCase testCase);
        void Release(ITestCaseProcessor processor);
    }
}
