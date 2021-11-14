using Qart.Testing.Framework;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineAction
    {
        Task ExecuteAsync(TestCaseContext testCaseContext);
    }
}
