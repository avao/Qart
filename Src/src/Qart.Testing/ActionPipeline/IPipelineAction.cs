using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineAction<T>
    {
        void Execute(TestCaseContext testCaseContext, T context);
    }
}
