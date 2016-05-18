using CommandLine;
using Common.Logging;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qart.CyberTester
{
    class CommandlineOptions
    {
        [Option('d', "dir", Required = false, HelpText = "Path to the directory(s) with testcase(s).")]
        public string Dir { get; set; }

        [Option('r', "rebaseline", Required = false, HelpText = "Overwrites expected results")]
        public bool Rebaseline { get; set; }

        [Option('o', "options", Required = false, HelpText = "Custom options in format '<name>=<value>;<name>=<value>'", DefaultValue = "")]
        public string Options { get; set; }

        [Option('h', "help", Required = false, HelpText = "Usage")]
        public bool Usage { get; set; }
    }

    class Program
    {
        static ILog Logger = LogManager.GetLogger("");

        static int Main(string[] args)
        {
            int result = -1;
            var options = new CommandlineOptions();
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                if (string.IsNullOrEmpty(options.Dir))
                {
                    options.Dir = Directory.GetCurrentDirectory();
                }
                result = Execute(options);
            }
            return result;
        }

        static int Execute(CommandlineOptions options)
        {
            Logger.DebugFormat("Rebaseline [{0}], TestCases [{1}]", options.Rebaseline, options.Dir);

            var container = Bootstrapper.CreateContainer();

            IContentProcessor contentProcessor = container.Resolve<IContentProcessor>();
            var testSystem = new TestSystem(new FileBasedDataStore(options.Dir), contentProcessor);

            var customSession = container.Kernel.HasComponent(typeof(ITestSession)) ? container.Resolve<ITestSession>() : null;

            var tester = new Qart.Testing.CyberTester(testSystem, container.Resolve<ITestCaseProcessorResolver>(), container.Resolve<ITestCaseLoggerFactory>(), container.Resolve<ILogManager>());

            var parsedOptions = options.Options.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(_ => _.SubstringBefore("="), _ => _.SubstringAfter("="));
            var results = tester.RunTests(customSession, parsedOptions);

            var failedTestsCount = results.Count(_ => _.Exception != null);
            Logger.InfoFormat("Tests execution finished. Number of failed testcases: {0}", failedTestsCount);

            XElement root = new XElement("TestResults", results.Select(_ => new XElement("Test", new XAttribute("id", _.TestCase.Id), new XAttribute("status", _.Exception == null ? "succeeded" : "failed"), _.Description == null ? null : _.Description.Root)));
            root.Save("TestSessionResults.xml");

            return failedTestsCount;
        }
    }
}
