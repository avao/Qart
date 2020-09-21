using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

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

        public ResolvableItemDescription(in ReadOnlySpan<char> actionName, Dictionary<string, object> parametersAsNvc)
        {
            Name = actionName.ToString();
            Parameters = parametersAsNvc;
        }
    }

}
