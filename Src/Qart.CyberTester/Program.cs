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

        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                if (string.IsNullOrEmpty(options.Dir))
                {
                    options.Dir = Directory.GetCurrentDirectory();
                }
            }
        }

        static void Execute(Options options)
        {
            var container = Bootstrapper.CreateContainer();

            var testSystem = new TestSystem(new Qart.Testing.FileBased.DataStore(options.Dir));
            var testCases = testSystem.GetTestCases();
            foreach (var testCase in testCases)
            {
                var feature = testCase.GetContent(".test");
                var processor = container.Resolve<ITestCaseProcessor>(feature);
                processor.Process(testCase);
            }
        }
    }
}
