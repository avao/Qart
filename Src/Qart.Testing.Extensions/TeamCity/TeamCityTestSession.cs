using JetBrains.TeamCity.ServiceMessages.Write;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Extensions.TeamCity
{
    public class TeamCityTestSession : ITestSession
    {
        private readonly ServiceMessageFormatter _formatter= new ServiceMessageFormatter();

        public void OnBegin(Framework.TestCaseContext ctx)
        {
            ctx.Logger.Info(_formatter.FormatMessage("testStarted", new ServiceMessageProperty("name", ctx.TestCase.Id)));
        }

        public void OnFinish(TestCaseResult result, Common.Logging.ILog logger)
        {
            if(result.Exception!=null)
            {
                if(result.IsMuted)
                {
                    logger.Info(_formatter.FormatMessage("testIgnored", new ServiceMessageProperty("name", result.TestCase.Id), new ServiceMessageProperty("message", "")));
                }
                else
                {
                    logger.Info(_formatter.FormatMessage("testFailed", new ServiceMessageProperty("name", result.TestCase.Id), new ServiceMessageProperty("message", result.Exception.Message), new ServiceMessageProperty("details", result.Exception.StackTrace)));
                }
            }
            logger.Info(_formatter.FormatMessage("testFinished", new ServiceMessageProperty("name", result.TestCase.Id)));
        }

        public void Dispose()
        {
        }
    }
}
