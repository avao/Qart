using Qart.Testing.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline
{
    public static class TestCaseExtensions
    {
        public static IEnumerable<ResolvableItemDescription> GetActionDescriptions(this TestCase testCase, ITestCaseProcessorFactory factory)
        {
            var processor = factory.GetProcessor(testCase);
            
            try
            {
                var pipelineProcessor = processor as IActionPipelineProcessor;
                if (pipelineProcessor != null)
                {
                    return pipelineProcessor.ActionDecsriptions;
                }

                return Enumerable.Empty<ResolvableItemDescription>();
            }
            finally
            {
                factory.Release(processor);
            }
        }
    }
}
