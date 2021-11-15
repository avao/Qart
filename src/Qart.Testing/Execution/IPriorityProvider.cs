using Qart.Testing.Framework;

namespace Qart.Testing.Execution
{
    public interface IPriorityProvider
    {
        int GetPriority(TestCase testCase);
    }
}
