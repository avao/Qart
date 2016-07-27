using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Framework
{
    public class TestCaseContext
    {
        public TestSession TestSession { get; private set; }
        public TestCase TestCase { get; private set; }
        public ILog Logger { get; private set; }
        public IDescriptionWriter DescriptionWriter { get; private set; }

        public TestCaseContext(TestSession testSession, TestCase testCase, ILog logger, IDescriptionWriter descriptionWriter)
        {
            TestSession = testSession;
            TestCase = testCase;
            Logger = logger;
            DescriptionWriter = descriptionWriter;
        }
    }
}
