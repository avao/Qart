using CommandLine;
using Common.Logging;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Testing;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Qart.CyberTester
{
    class CommandlineOptions
    {
        [Option('d', "dir", Required = false, HelpText = "Path to the directory(s) with testcase(s).")]
        public string Dir { get; set; }

        [Option('r', "rebase", Required = false, HelpText = "Specifies whether to update expectations with actual data or not.")]
        public bool Rebase { get; set; }

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
            if (Parser.Default.ParseArgumentsStrict(args, options))
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
            Logger.DebugFormat("TestCases [{0}]", options.Dir);

            var container = Bootstrapper.CreateContainer(new FileBasedDataStore(options.Dir));

            var parsedOptions = options.Options.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(_ => _.LeftOf("="), _ => _.RightOf("="));
            parsedOptions.Add("ct.dir", options.Dir);
            parsedOptions.Add("ct.rebase", options.Rebase ? bool.TrueString : bool.FalseString);

            var results = container.Resolve<Testing.CyberTester>().RunTests(container.ResolveAll<ITestSession>(), parsedOptions).ToList();

            var failedTestsCount = results.Count(_ => _.Exception != null);
            var nonMutedfailedTestsCount = results.Count(_ => _.Exception != null && !_.IsMuted);
            Logger.InfoFormat("Tests execution finished. Testcases: {0}, total failures: {1}, non-muted failures: {2}", results.Count, failedTestsCount, nonMutedfailedTestsCount);

            XElement root = new XElement("TestResults", results.Select(_ => new XElement("Test", new XAttribute("id", _.TestCase.Id), new XAttribute("status", _.Exception == null ? "succeeded" : "failed"), new XAttribute("muted", _.IsMuted), _.Description == null ? null : _.Description.Root)));
            root.Save("TestSessionResults.xml");

            return nonMutedfailedTestsCount;
        }
    }
}
