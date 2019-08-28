using System;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class ResolvableItemDescription
    {
        public string Name { get; private set; }
        public IDictionary<string, object> Parameters { get; private set; }

        public ResolvableItemDescription(string name, IDictionary<string, object> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }

}
