using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class ResolvableItemDescription
    {
        public string Name { get; }
        public IDictionary<string, object> Parameters { get; }

        public ResolvableItemDescription(string name, IDictionary<string, object> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
