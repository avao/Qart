using Common.Logging;
using Qart.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestCaseLoggerFactory : ITestCaseLoggerFactory
    {
        private readonly ILogManager _logManager;
        private readonly ILog _log;
        public TestCaseLoggerFactory(ILogManager logManager)
        {
            _logManager = logManager;
            _log = logManager.GetLogger("");
        }

        public IDisposableLogger GetLogger(TestCase testCase)
        {
            var logger = _logManager.GetLogger("TestCaseLogger");

            var stream = testCase.GetWriteStream("execution.log");
            var writer = new StreamWriter(stream);

            return new CompositeLogger(new[] {   
                new CompositeLogger.LoggerInfo(_log, true), 
                new CompositeLogger.LoggerInfo(new TextWriterLogger("TestCaseLogger", LogLevel.All, true, true, true, "yyyy-MM-dd hh:mm:ss", writer), false) });

        }
    }
}
