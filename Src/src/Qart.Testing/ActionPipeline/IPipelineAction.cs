using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineAction<in T>
        where T : IPipelineContext
    {
        void Execute(TestCaseContext testCaseContext, T context);
    }
}
