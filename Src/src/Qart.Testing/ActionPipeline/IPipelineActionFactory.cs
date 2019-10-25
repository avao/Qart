using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineActionFactory
    {
        IPipelineAction Create(IDictionary<string, object> arguments);
        IPipelineAction Get(string name, IDictionary<string, object> arguments);
        void Release(IPipelineAction action);
    }
}
