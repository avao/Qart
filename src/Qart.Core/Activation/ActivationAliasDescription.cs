using System.Collections.Generic;
using System.ComponentModel;

namespace Qart.Core.Activation
{
    public class ActivationAliasDescription
    {
        public string Name { get; }
        public string Description { get; }

        public IReadOnlyCollection<IReadOnlyCollection<ParameterDescription>> ParameterGroups { get; }

        public ActivationAliasDescription(string name, string description, IReadOnlyCollection<IReadOnlyCollection<ParameterDescription>> parameterGroups)
        {
            Name = name;
            Description = description;
            ParameterGroups = parameterGroups;
        }

        public class ParameterDescription
        {
            public string Name { get; }
            public string Description { get; }

            [DefaultValue("String")]
            public string Type { get; }

            [DefaultValue(false)]
            public bool IsOptional { get; }
            [DefaultValue(null)]
            public object DefaultValue { get; }

            public ParameterDescription(string name, string description, string type, bool isOptional, object defaultValue)
            {
                Name = name;
                Description = description;
                Type = type;
                IsOptional = isOptional;
                DefaultValue = defaultValue;
            }
        }
    }
}
