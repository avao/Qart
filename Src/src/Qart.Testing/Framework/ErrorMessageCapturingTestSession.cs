using Common.Logging;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

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

        public void OnFinish(TestCaseResult result, ILog logger)
        {
            if (result.Exception != null)
            {
                var aggregateException = result.Exception as AggregateException;
                var messages = aggregateException == null ? new[] { result.Exception.Message } : aggregateException.InnerExceptions.Select(_ => _.Message);
                result.Description.Root.Add(new XElement("ErrorMessage", string.Join(Environment.NewLine, messages)));
            }
        }

        public void Dispose()
        {
        }
    }
}
