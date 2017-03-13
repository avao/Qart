using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public interface IActionPipelineProcessor : ITestCaseProcessor
    {
        IEnumerable<ResolvableItemDescription> ActionDecsriptions { get; }
    }
}
