using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qart.Testing
{
    public interface ITestCaseProcessor //TODO separate assembly
    {
        void Process(TestCase testCase);
    }
}
