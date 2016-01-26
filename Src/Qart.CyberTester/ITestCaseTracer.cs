using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public interface ITestCaseTracer
    {
        void OnBegin(TestCase testCase);
        void OnFailure(TestCase testCase, Exception ex);
        void OnFinish(TestCase testCase);
    }
}
