using Castle.Facilities.TypedFactory;
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

        protected override System.Collections.IDictionary GetArguments(MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 1)
            {
                var dictionary = arguments[0] as System.Collections.IDictionary;
                if (dictionary != null)
                    return dictionary;
            }
            else if (method.Name.ToLower().StartsWith("get") && arguments.Length == 2)
            {
                var dictionary = arguments[1] as System.Collections.IDictionary;
                if (dictionary != null)
                    return dictionary;
            }
            return base.GetArguments(method, arguments);
        }
    }
}
