using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface ITestSession : IDisposable
    {
        //instead of having explicit Setup and TearDown one can use c-tor and Dispose

        void OnBegin(TestCase testCase);
        void OnFinish(TestCaseResult result);
    }

}
