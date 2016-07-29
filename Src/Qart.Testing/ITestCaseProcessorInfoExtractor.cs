using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Testing
{    
    public interface ITestCaseProcessorInfoExtractor
    {
        TestCaseProcessorInfo Execute(TestCase testCase);
    }
}
