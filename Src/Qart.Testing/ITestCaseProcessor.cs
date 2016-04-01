using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Qart.Testing
{
    public interface ITestCaseProcessor //TODO separate assembly
    {
        void Process(TestSession testSession, TestCase testCase, ILog logger);
        XDocument GetDescription(TestCase testCase);
    }
}
