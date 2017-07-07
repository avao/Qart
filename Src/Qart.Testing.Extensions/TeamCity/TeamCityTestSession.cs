using JetBrains.TeamCity.ServiceMessages.Write;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Qart.Testing.Extensions.TeamCity
{
    public class TeamCityTestSession : ITestSession
    {
        private readonly ServiceMessageFormatter _formatter = new ServiceMessageFormatter();

        public void OnSessionStart(IDictionary<string, string> options)
        {
        }

        public void OnBegin(Framework.TestCaseContext ctx)
        {
            ctx.Logger.Info(_formatter.FormatMessage("testStarted", GetCommonProperties(ctx.TestCase.Id)));
        }

        public void OnFinish(TestCaseResult result, Common.Logging.ILog logger)
        {
            var commonProperties = GetCommonProperties(result.TestCase.Id);
            if (result.Exception != null)
            {
                if (result.IsMuted)
                {
                    logger.Info(_formatter.FormatMessage("testIgnored", commonProperties.Concat(new[] { new ServiceMessageProperty("message", "") })));
                }
                else
                {
                    logger.Info(_formatter.FormatMessage("testFailed", commonProperties.Concat(new[] { new ServiceMessageProperty("message", result.Exception.Message), new ServiceMessageProperty("details", result.Exception.StackTrace) })));
                }
            }
            logger.Info(_formatter.FormatMessage("testFinished", commonProperties));
        }

        public void Dispose()
        {
        }

        private IEnumerable<ServiceMessageProperty> GetCommonProperties(string testId)
        {
            yield return new ServiceMessageProperty("name", testId);
            yield return new ServiceMessageProperty("flowId", testId);
        }
    }
}
