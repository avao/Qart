using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Testing;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Qart.CyberTester
{
    public class CommandlineOptions
    {
        [Option('d', "dir", Required = false, HelpText = "Path to the directory(s) with testcase(s).")]
        public string Dir { get; set; }

        [Option('r', "rebase", Required = false, HelpText = "Specifies whether to update expectations with actual data or not.")]
        public bool Rebase { get; set; }

        [Option('s', "suppressExceptions", Required = false, HelpText = "Specifies whether to defer exceptions while executing actions to the end.")]
        public bool SuppressExceptions { get; set; }

        [Option('l', "logLevel", Required = false, HelpText = "Log level", Default = LogEventLevel.Information)]
        public LogEventLevel LogLevel { get; set; }


        [Option('o', "options", Required = false, HelpText = "Custom options in format '<name>=<value>;<name>=<value>'")]
        public string Options { get; set; }

        [Option('h', "help", Required = false, HelpText = "Usage")]
        public bool Usage { get; set; }
    }

    public class Program
    {
        public static int Main(string[] args)
        {
            int result = -1;
            var parserResult = Parser.Default.ParseArguments<CommandlineOptions>(args);
            if (parserResult.Tag == ParserResultType.Parsed)
            {
                CommandlineOptions parsedOptions = null;
                parserResult.WithParsed(options => parsedOptions = options);
                if (string.IsNullOrEmpty(parsedOptions.Dir))
                {
                    parsedOptions.Dir = Directory.GetCurrentDirectory();
                }

                result = Execute(parsedOptions);
            }
            return result;
        }

        public static int Execute(CommandlineOptions options)
        {
            var serviceProvider = Bootstrapper.CreateContainer(new FileBasedDataStore(options.Dir), new ServiceCollection(), options.LogLevel);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("TestCases [{0}]", options.Dir);

            var parsedOptions = options.Options?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(_ => _.LeftOf("="), _ => _.RightOf("=")) ?? new Dictionary<string, string>();
            parsedOptions.Add("ct.dir", options.Dir);
            parsedOptions.Add("ct.rebase", options.Rebase ? bool.TrueString : bool.FalseString);
            parsedOptions.Add("ct.deferExceptions", options.SuppressExceptions ? bool.TrueString : bool.FalseString);

            var results = serviceProvider.GetRequiredService<Testing.CyberTester>().RunTests(serviceProvider.GetServices<ITestSession>(), parsedOptions).ToList();

            var failedTestsCount = results.Count(_ => _.Exception != null);
            var nonMutedfailedTestsCount = results.Count(_ => _.Exception != null && !_.IsMuted);
            logger.LogInformation("Tests execution finished. Testcases: {0}, total failures: {1}, non-muted failures: {2}", results.Count, failedTestsCount, nonMutedfailedTestsCount);

            XElement root = new XElement("TestResults", results.Select(_ => new XElement("Test", new XAttribute("id", _.TestCase.Id), new XAttribute("status", _.Exception == null ? "succeeded" : "failed"), new XAttribute("muted", _.IsMuted), _.Description == null ? null : _.Description.Root)));
            root.Save("TestSessionResults.xml");

            return nonMutedfailedTestsCount;
        }
    }
}
