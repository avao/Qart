using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Qart.Testing.Framework;
using System.Reflection;

namespace Qart.Testing.Extensions.Windsor
{
    public class TestCaseProcessorTypedFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private static TestCaseProcessorInfoExtractor _parametersExtractor = new TestCaseProcessorInfoExtractor();

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            return arguments.Length == 1 && arguments[0] is TestCase testCase
                ? _parametersExtractor.Execute(testCase).Name
                : base.GetComponentName(method, arguments);
        }

        protected override Arguments GetArguments(MethodInfo method, object[] arguments)
        {
            return arguments.Length == 1 && arguments[0] is TestCase testCase
                ? Arguments.FromNamed(_parametersExtractor.Execute(testCase).Parameters)
                : base.GetArguments(method, arguments);
        }
    }
}
