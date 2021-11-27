using Qart.Core.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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

    public static class ActivationAliasDescriptionExtensions
    {
        public static string ToShortDescription(this ActivationAliasDescription description)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(description.Name);
            foreach (var parameterGroup in description.ParameterGroups)
            {
                var parameters = parameterGroup.Select(p => (p.IsOptional ? "+" : "") + p.Name).ToCsvWithASpace();
                stringBuilder.AppendLine($"\t{parameters}");
            }
            return stringBuilder.ToString();
        }
    }
}