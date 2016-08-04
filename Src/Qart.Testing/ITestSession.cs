using Common.Logging;
using Qart.Testing.Framework;
using System;

namespace Qart.Testing
{
    public interface ITestSession : IDisposable
    {
        //instead of having explicit Setup and TearDown one can use c-tor and Dispose
        //TODO add dedicated functions instead

        void OnBegin(TestCaseContext ctx);
        void OnFinish(TestCaseResult result, ILog logger);
    }

}
