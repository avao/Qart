namespace Qart.Testing
{
    public interface ITestCaseFilter
    {
        bool ShouldProcess(TestCase testCase);
    }
}
