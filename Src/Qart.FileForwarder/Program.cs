using CommandLine;
using Qart.Core.Io.FileRolling;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.FileForwarder
{
    class Options
    {
        [Option('r', "rabbiturl", Required = true, HelpText = "host:port/exchange")]
        public string RabbitUrl { get; set; }

        [Option('i', "input", Required = true, HelpText = "File pattern to be forwarded.")]
        public string FilePattern { get; set; }

        [Option('p', "positionstoredir", Required = true, HelpText = "PositionStore folder.")]
        public string PositionStoreDir { get; set; }
    }

    class Program
    {
        // -r localhost:5672/exchange -i D:\Work\Qart\Src\Qart.RandomLogger\bin\Debug\log.txt -p c:\work\output
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
                var bits = options.RabbitUrl.Split(new[] { ':', '/' });
                var publisher = new RabbitMqPublisher(bits[0], int.Parse(bits[1]), "publisher", "publisher", bits[2]);

                string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
                var publisher = new RabbitMqPublisher("localhost", 5672, "publisher", "publisher", "exchange");
                var manager = new RollingFileTextReaderManager(options.FilePattern,
                                                            new FileBasedPositionStore(options.PositionStoreDir),
                                                            ReadBehaviour.FromWhereLeft,
                                                            (content, meta) => { publisher.Publish(content, computerName + "." + "AppName" + "." + meta); return true; });

                Console.ReadKey();
            }
        }

        static bool ProcessLine(string fileName, string line)
        {
            Console.WriteLine(fileName + ": " + line);
            return true;
        }
    }
}
