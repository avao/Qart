using JetBrains.TeamCity.ServiceMessages.Write;
using Microsoft.Extensions.Logging;
using Qart.Core.Text;
using Qart.Testing.Execution;
using Qart.Testing.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qart.Testing.Extensions.TeamCity
{
    public class TeamCityTestSession : ITestSession
    {
        private readonly ServiceMessageFormatter _formatter = new();

        public void OnSessionStart(IDictionary<string, string> options)
        {
        }

        public void OnBegin(TestCaseContext ctx)
        {
            ctx.Logger.LogInformation(_formatter.FormatMessage("testStarted", GetCommonProperties(ctx.TestCase.Id)));
        }

        public void OnFinish(TestCaseExecutionResult result, ILogger logger)
        {
            var commonProperties = GetCommonProperties(result.TestCase.Id);
            if (result.Exception != null)
            {

                if (result.IsMuted)
                {
                    logger.LogInformation(_formatter.FormatMessage("testIgnored", commonProperties.Concat(new[] { new ServiceMessageProperty("message", "") })));
                }
                else
                {
                    var detailsBuilder = new StringBuilder(result.Exception.StackTrace);
                    if (result.Exception is AssertException assertException && assertException.TryGetCategories(out var categories))
                    {
                        detailsBuilder.AppendLine();
                        detailsBuilder.Append("Categories: " + categories.ToCsv());
                    }

                    foreach (DictionaryEntry item in result.Exception.Data)
                    {
                        detailsBuilder.AppendLine();
                        detailsBuilder.Append(item.Key);
                        detailsBuilder.Append(" : ");
                        detailsBuilder.Append(item.Value);
                    }
                    logger.LogInformation(_formatter.FormatMessage("testFailed", commonProperties.Concat(new[] { new ServiceMessageProperty("message", result.Exception.Message), new ServiceMessageProperty("details", detailsBuilder.ToString()) })));
                }
            }
            logger.LogInformation(_formatter.FormatMessage("testFinished", commonProperties));
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
