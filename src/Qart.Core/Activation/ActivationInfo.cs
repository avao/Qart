using System;
using System.Collections.Generic;

namespace Qart.Core.Activation
{
    public class ActivationInfo
    {
        public Type Type { get; }
        public IDictionary<string, object> Parameters { get; }

        public ActivationInfo(Type type, IDictionary<string, object> parameters)
        {
            Type = type;
            Parameters = parameters;
        }

        public static ActivationInfo Create<T>(IDictionary<string, object> parameters)
            => new ActivationInfo(typeof(T), parameters);
    }
}
