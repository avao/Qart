using System;
using System.Collections.Generic;

namespace Qart.Testing.TestCasesPreprocessors
{
    public class FuncBasedTestCasesPreprocessor : ITestCasesPreprocessor
    {
        private readonly Func<IEnumerable<TestCase>, IDictionary<string, string>, IEnumerable<TestCase>> _processingFunc;
        public FuncBasedTestCasesPreprocessor(Func<IEnumerable<TestCase>, IDictionary<string, string>, IEnumerable<TestCase>> processingFunc)
        {
            _processingFunc = processingFunc;
        }

        public IEnumerable<TestCase> Execute(IEnumerable<TestCase> testCases, IDictionary<string, string> options)
        {
            return _processingFunc(testCases, options);
        }
    }
}
