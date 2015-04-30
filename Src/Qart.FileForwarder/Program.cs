using Qart.Core.Io.FileRolling;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.FileForwarder
{
    class Program
    {
        static void Main(string[] args)
        {

            string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            var publisher = new RabbitMqPublisher("localhost", 5672, "publisher", "publisher",  "exchange");

            var manager = new RollingFileTextReaderManager(@"D:\Work\Qart\Src\Qart.RandomLogger\bin\Debug\log.txt",
                                                        new FileBasedPositionStore(@"c:\work\output"),
                                                        ReadBehaviour.FromWhereLeft,
                                                        (meta, content) => { publisher.Publish(content, computerName + "." + "AppName" + "." + meta); return true; });


            Console.ReadKey();
        }

        static bool ProcessLine(string fileName, string line)
        {
            Console.WriteLine(fileName + ": " + line);
            return true;
        }
    }
}
