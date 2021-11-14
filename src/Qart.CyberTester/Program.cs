using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qart.Core.Activation;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Execution;
using Qart.Testing.Framework.Json;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qart.CyberTester
{
    public class Program
    {
        interface IExecutionOptions
        {
            [Option('d', "dir", Required = false, HelpText = "Path to the directory(s) with testcase(s).", SetName = "execution")]
            public string Dir { get; set; }

            [Option('r', "rebase", Required = false, HelpText = "Specifies whether to update expectations with actual data or not.", SetName = "execution")]
            public bool Rebase { get; set; }

            [Option('s', "suppressExceptions", Required = false, HelpText = "Specifies whether to defer exceptions while executing actions to the end.", SetName = "execution")]
            public bool SuppressExceptions { get; set; }

            [Option('l', "logLevel", Required = false, HelpText = "Log level", Default = LogEventLevel.Information, SetName = "execution")]
            public LogEventLevel LogLevel { get; set; }


            [Option('o', "options", Required = false, HelpText = "Custom options in format '<name>=<value>;<name>=<value>'", SetName = "execution")]
            public string Options { get; set; }
        }

        interface IQueryOptions
        {
            [Option('a', "actions", Required = false, HelpText = "Prints information about available action and parameters", SetName = "query")]
            public bool Actions { get; set; }
            [Option('c', "compressed", Required = false, HelpText = "Specifies that action information should be printed in compressed form", SetName = "query")]
            public bool ShortFormat { get; set; }

        }

        private class CommandlineOptions : IExecutionOptions, IQueryOptions
        {
            public string Dir { get; set; }
            public bool Rebase { get; set; }
            public bool SuppressExceptions { get; set; }
            public LogEventLevel LogLevel { get; set; }
            public string Options { get; set; }


            public bool Actions { get; set; }
            public bool ShortFormat { get; set; }
        }

        public static Task<int> Main(string[] args)
        {
            return ExecuteAsync(args);
        }

        public static Task<int> ExecuteAsync(string[] args, Action<IServiceCollection> customServiceRegistrationAction = null, Action<ActivationRegistry<IPipelineAction>> customActionRegistrationAction = null)
        {
            return Parser.Default.ParseArguments<CommandlineOptions>(args)
                .MapResult(async parsedOptions =>
                {
                    if (parsedOptions.Actions)
                    {
                        var serviceProvider = CreateServiceProvider(parsedOptions.Dir, parsedOptions.LogLevel, customServiceRegistrationAction, customActionRegistrationAction);
                        DescribeActions(serviceProvider, parsedOptions.ShortFormat);
                        return 0;
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(parsedOptions.Dir))
                        {
                            parsedOptions.Dir = Directory.GetCurrentDirectory();
                        }

                        var extraOptions = parsedOptions.Options?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(_ => _.LeftOf("="), _ => _.RightOf("=")) ?? new Dictionary<string, string>();

                        return await ExecuteAsync(parsedOptions.Dir, parsedOptions.Rebase, parsedOptions.SuppressExceptions, parsedOptions.LogLevel, extraOptions, customServiceRegistrationAction, customActionRegistrationAction);
                    }
                },
                err => Task.FromResult(-1));
        }

        public static IServiceProvider CreateServiceProvider(string dir, LogEventLevel? logLevel = null, Action<IServiceCollection> customServiceRegistrationAction = null, Action<ActivationRegistry<IPipelineAction>> customActionRegistrationAction = null)
        {
            var services = new ServiceCollection();
            var actionsRegistry = Bootstrapper.RegisterStandardServices(services, new FileBasedDataStore(dir), logLevel ?? LogEventLevel.Information);
            customServiceRegistrationAction?.Invoke(services);
            customActionRegistrationAction?.Invoke(actionsRegistry);

            return services.BuildServiceProvider();
        }


        public static void DescribeActions(IServiceProvider serviceProvider, bool compressed)
        {
            var actionFactory = serviceProvider.GetRequiredService<IObjectFactory<IPipelineAction>>();
            var descriptions = actionFactory.GetDescriptions();
            if (compressed)
            {
                foreach (var aliasDescription in descriptions)
                {
                    Console.WriteLine();
                    Console.WriteLine(aliasDescription.Name);
                    foreach (var parameterGroup in aliasDescription.ParameterGroups)
                    {
                        var parameters = parameterGroup.Select(p => (p.IsOptional ? "+" : "") + p.Name).ToCsvWithASpace();
                        Console.WriteLine($"\t{parameters}");
                    }
                }
            }
            else
            {
                Console.WriteLine(descriptions.ToIndentedJsonNoDefaults());
            }
        }

        public static async Task<int> ExecuteAsync(string dir, bool rebase, bool suppressExceptions, LogEventLevel logLevel, IDictionary<string, string> extraOptions, Action<IServiceCollection> customServiceRegistrationAction, Action<ActivationRegistry<IPipelineAction>> customActionRegistrationAction)
        {
            var serviceProvider = CreateServiceProvider(dir, logLevel, customServiceRegistrationAction, customActionRegistrationAction);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("TestCases [{0}]", dir);

            var options = new Dictionary<string, string>(extraOptions)
            {
                { "ct.dir", dir },
                { "ct.rebase", rebase ? bool.TrueString : bool.FalseString },
                { "ct.deferExceptions", suppressExceptions ? bool.TrueString : bool.FalseString }
            };

            var results = await serviceProvider.GetRequiredService<Testing.CyberTester>().RunTestsAsync(serviceProvider.GetService<ITestSession>(), options);

            var failedTestsCount = results.Count(_ => _.Exception != null);
            var nonMutedfailedTestsCount = results.Count(_ => _.Exception != null && !_.IsMuted);
            logger.LogInformation("Tests execution finished. Testcases: {0}, total failures: {1}, non-muted failures: {2}", results.Count, failedTestsCount, nonMutedfailedTestsCount);

            XElement root = new XElement("TestResults", results.Select(_ => new XElement("Test", new XAttribute("id", _.TestCase.Id), new XAttribute("status", _.Exception == null ? "succeeded" : "failed"), new XAttribute("muted", _.IsMuted), _.Description == null ? null : _.Description.Root)));
            root.Save("TestSessionResults.xml");

            return nonMutedfailedTestsCount;
        }
    }
}
