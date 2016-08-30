using Castle.Facilities.TypedFactory;
using Qart.Testing.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Qart.Testing.Extensions.Windsor
{
    public class TestCaseProcessorTypedFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private static TestCaseProcessorInfoExtractor _parametersExtractor = new TestCaseProcessorInfoExtractor();

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 1)
            {
                var testCase = arguments[0] as TestCase;
                if (testCase != null)
                {
                    return _parametersExtractor.Execute(testCase).Name;
                }
            }
            return base.GetComponentName(method, arguments);
        }

        protected override IDictionary GetArguments(System.Reflection.MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 1)
            {
                var testCase = arguments[0] as TestCase;
                if (testCase != null)
                {
                    return (IDictionary)_parametersExtractor.Execute(testCase).Parameters;
                }
            }
            return base.GetArguments(method, arguments);
        }
    }
}
