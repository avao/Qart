using System;
using Qart.Testing;

namespace Qart.Wheels.TestAutomation.TestCaseProcessors
{
    public class ThrowAnExceptionProcessor : ITestCaseProcessor
    {
        public void Process(Testing.Framework.TestCaseContext c)
        {
            c.DescriptionWriter.AddNote("Action", "started");
            c.Logger.InfoFormat("About to throw from {0}", c.TestCase.Id);
            throw new Exception("an exception");
        }
    }
}
