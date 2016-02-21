using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface ITestCaseProcessorResolver
    {
        ITestCaseProcessor Resolve(TestCase testCase);
    }
       
}
