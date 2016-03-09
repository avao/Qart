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
        public void OnBegin(TestCase testCase, ILog logger)
        {
            logger.DebugFormat("Starting processing test case [{0}]", testCase.Id);
        }

        public void OnFinish(TestCaseResult result, ILog logger)
        {
            if(result.Exception != null)
            {
                logger.Error("An error occured:", result.Exception);
            }
            logger.DebugFormat("Finished processing test case [{0}]", result.TestCase.Id);
        }

        public void Dispose()
        {
        }
    }
}
