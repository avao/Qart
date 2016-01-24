using CommandLine;
using Common.Logging;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    class Options
    {
        [Option('d', "dir", Required = false, HelpText = "Path to the directory(s) with testcase(s).")]
        public string Dir { get; set; }

        [Option('r', "rebaseline", Required = false, HelpText = "Overwrites expected results")]
        public bool Rebaseline { get; set; }

        [Option('h', "help", Required = false, HelpText = "Usage")]
        public bool Usage { get; set; }
    }

    class Program
    {
        static ILog Logger = LogManager.GetLogger("");

        static int Main(string[] args)
        {
            int result = -1;
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                if (string.IsNullOrEmpty(options.Dir))
                {
                    options.Dir = Directory.GetCurrentDirectory();
                }
                result = Execute(options);
                Logger.InfoFormat("Failed testcases: {0}", result);
            }
            return result;
        }

        static int Execute(Options options)
        {
            Logger.DebugFormat("Rebaseline [{0}], TestCases [{1}]", options.Rebaseline, options.Dir);

            var container = Bootstrapper.CreateContainer();

            var testSystem = new TestSystem(new Qart.Testing.FileBased.DataStore(options.Dir));

            int failedTests = 0;
            Logger.Debug("Looking for test cases.");
            var testCases = testSystem.GetTestCases();
            foreach (var testCase in testCases)
            {
                Logger.DebugFormat("Starting processing test case [{0}]", testCase.Id);
                try
                {
                    var feature = testCase.GetContent(".test");
                    var processor = container.Resolve<ITestCaseProcessor>(feature);
                    processor.Process(testCase);
                }
                catch(Exception ex)
                {
                    ++failedTests;
                    Logger.Error(string.Format("An exception was raised while processing [{0}]", testCase.Id), ex);
                }
                Logger.DebugFormat("Finished processing test case [{0}]", testCase.Id);
            }

            return failedTests;
        }
    }
}
