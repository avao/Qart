using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineActionFactory<T>
    {
        IPipelineAction<T> Create(IDictionary<string, object> arguments);
        IPipelineAction<T> Get(string name, IDictionary<string, object> arguments);
        void Release(IPipelineAction<T> action);
    }
}
