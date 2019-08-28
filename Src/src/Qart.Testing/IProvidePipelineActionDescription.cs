using System.Collections.Generic;

namespace Qart.Testing
{
    public class ActionParameterMetaData
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool IsOptional { get; private set; }

        public ActionParameterMetaData(string name, string type, bool isOptional)
        {
            Name = name;
            Type = type;
        }
    }

    public class ActionMetaData
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<ActionParameterMetaData> Parameters { get; private set; }

        public ActionMetaData(string name, string description, IEnumerable<ActionParameterMetaData> parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }
    }
 

    public interface IProvidePipelineActionDescription
    {
        ActionMetaData MetaData { get; }
    }
}
