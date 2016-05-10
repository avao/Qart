namespace Qart.Testing
{
    public interface ITestCaseProcessorResolver
    {
        ITestCaseProcessor Resolve(TestCase testCase);
    }
}
