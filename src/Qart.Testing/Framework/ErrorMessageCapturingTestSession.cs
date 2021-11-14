using Microsoft.Extensions.Logging;
using Qart.Core.Text;
using Qart.Testing.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qart.Testing.Framework
{
    public class ErrorMessageCapturingTestSession : ITestSession
    {
        public void OnSessionStart(IDictionary<string, string> options)
        {
        }

        public void OnBegin(TestCaseContext ctx)
        {
        }

        public void OnFinish(TestCaseExecutionResult result, ILogger logger)
        {
            if (result.Exception != null)
            {
                var aggregateException = result.Exception as AggregateException;
                var messages = aggregateException == null ? new[] { result.Exception.Message } : aggregateException.InnerExceptions.Select(_ => _.Message);
                result.Description.Root.Add(new XElement("ErrorMessage", messages.ToMultiLine()));
            }
        }

        public void Dispose()
        {
        }
    }
}
