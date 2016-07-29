using System;
using System.Collections.Generic;

namespace Qart.Testing.Framework
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

}
