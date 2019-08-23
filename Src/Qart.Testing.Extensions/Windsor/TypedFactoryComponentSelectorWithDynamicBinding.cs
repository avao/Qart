using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qart.Testing.Extensions.Windsor
{
    public class TypedFactoryComponentSelectorWithDynamicBinding : DefaultTypedFactoryComponentSelector
    {
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (method.Name.ToLower().StartsWith("get") && arguments.Length >= 1)
            {
                var name = arguments[0] as string;
                if (name != null)
                {
                    return name;
                }
            }
            return base.GetComponentName(method, arguments);
        }

        protected override Arguments GetArguments(MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 1)
            {
                if (arguments[0] is IDictionary<string, object> dictionary)
                    return Arguments.FromNamed(dictionary);
            }
            else if (method.Name.ToLower().StartsWith("get") && arguments.Length == 2)
            {
                if (arguments[1] is IDictionary<string, object> dictionary)
                    return Arguments.FromNamed(dictionary);
            }
            return base.GetArguments(method, arguments);
        }
    }
}
