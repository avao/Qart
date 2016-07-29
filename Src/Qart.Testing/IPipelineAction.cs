using Qart.Testing.Framework;

namespace Qart.Testing
{
    public interface IPipelineAction<T>
    {
        void Execute(TestCaseContext testCaseContext, T context);
    }
}
