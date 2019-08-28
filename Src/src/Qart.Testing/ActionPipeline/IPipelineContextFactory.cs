using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineContextFactory<T>
    {
        T CreateContext(TestCaseContext c);
        void Release(T context);
    }
}
