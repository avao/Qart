using Castle.Facilities.TypedFactory;

namespace Qart.Testing.Extensions.Windsor
{
    public class TypedFactoryComponentSelectorWithDynamicBinding : DefaultTypedFactoryComponentSelector
    {
        protected override System.Collections.IDictionary GetArguments(System.Reflection.MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 1)
            {
                var dictionary = arguments[0] as System.Collections.IDictionary;
                if (dictionary != null)
                    return dictionary;
            }
            return base.GetArguments(method, arguments);
        }
    }
}
