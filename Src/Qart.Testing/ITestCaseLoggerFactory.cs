using Common.Logging;
using Qart.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface ITestCaseLoggerFactory
    {
        IDisposableLogger GetLogger(TestCase testCase);
    }
}
