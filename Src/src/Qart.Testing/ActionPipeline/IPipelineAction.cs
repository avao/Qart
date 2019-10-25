using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineAction
    {
        void Execute(TestCaseContext testCaseContext);
    }
}
