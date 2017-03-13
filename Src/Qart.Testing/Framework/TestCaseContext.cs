using Common.Logging;
using Qart.Core.Logging;
using System;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class TestCaseContext : IDisposable
    {
        public TestCase TestCase { get; private set; }
        public IDisposableLogger Logger { get; private set; }
        public IDescriptionWriter DescriptionWriter { get; private set; }
        public IDictionary<string, string> Options {get; private set;}

        public TestCaseContext(IDictionary<string, string> options, TestCase testCase, IDisposableLogger logger, IDescriptionWriter descriptionWriter)
        {
            Options = options;
            TestCase = testCase;
            Logger = logger;
            DescriptionWriter = descriptionWriter;
        }

        public void Dispose()
        {
            Logger.Dispose();
        }
    }
}
