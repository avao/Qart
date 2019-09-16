using Microsoft.Extensions.Logging;
using Qart.Testing.Framework.Logging;
using System.IO;

namespace Qart.Testing.Framework
{
    public class TestCaseLoggerFactory : ITestCaseLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _log;
        public TestCaseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _log = loggerFactory.CreateLogger("");
        }

        public IDisposableLogger GetLogger(TestCase testCase)
        {
            var logger = _loggerFactory.CreateLogger("TestCaseLogger");

            var stream = testCase.GetWriteStream("execution.log");
            var writer = new StreamWriter(stream);

            return new CompositeLogger(new[] {
                new CompositeLogger.LoggerInfo(_log, false),
                new CompositeLogger.LoggerInfo(new TextWriterLogger("TestCaseLogger", LogLevel.Debug, writer), true)
            });
        }
    }
}
