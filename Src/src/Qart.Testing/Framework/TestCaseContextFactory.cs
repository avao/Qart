using Qart.Testing.ActionPipeline;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class TestCaseContextFactory : ITestCaseContextFactory
    {
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly IItemProvider _itemsInitialiser;

        public TestCaseContextFactory(ITestCaseLoggerFactory testCaseLoggerFactory, IItemProvider itemsInitialiser)
        {
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _itemsInitialiser = itemsInitialiser;
        }

        public TestCaseContext CreateContext(TestCase testCase, IDictionary<string, string> options)
        {
            var testCaseLogger = _testCaseLoggerFactory.GetLogger(testCase);
            return new TestCaseContext(options, testCase, testCaseLogger, new XDocumentDescriptionWriter(testCaseLogger), new ItemsHolder(_itemsInitialiser));
        }

        public void Release(TestCaseContext context)
        {
            context.Dispose();
        }
    }
}
