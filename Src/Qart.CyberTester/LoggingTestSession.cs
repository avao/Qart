using Common.Logging;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class LoggingTestSession : ITestSession
    {
        private readonly ILog _log;

        public LoggingTestSession(ILogManager logManager)
        {
            _log = logManager.GetLogger("");
        }

        public void OnBegin(TestCase testCase)
        {
            _log.DebugFormat("Starting processing test case [{0}]", testCase.Id);
        }

        public void OnFinish(TestCaseResult result)
        {
            if(result.Exception != null)
            {
                _log.Error("An error occured:", result.Exception);
            }
            _log.DebugFormat("Finished processing test case [{0}]", result.TestCase.Id);
        }

        public void Dispose()
        {
        }
    }
}
