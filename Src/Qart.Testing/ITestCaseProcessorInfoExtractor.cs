using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestCaseProcessorInfo
    {
        public string ProcessorId { get; private set; }
        public IDictionary<string, object> Parameters { get; private set; }

        public TestCaseProcessorInfo(string processorId, IDictionary<string, object> parameters)
        {
            ProcessorId = processorId;
            Parameters = parameters;
        }
    }

    public interface ITestCaseProcessorInfoExtractor
    {
        TestCaseProcessorInfo Execute(TestCase testCase);
    }
}
