using Microsoft.Extensions.Logging;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ITestSession : IDisposable
    {
        void OnSessionStart(IDictionary<string, string> options);

        void OnBegin(TestCaseContext ctx);
        void OnFinish(TestCaseResult result, ILogger logger);
    }
}
