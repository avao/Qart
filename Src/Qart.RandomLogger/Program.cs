using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Linq;

namespace Qart.RandomLogger
{
    class Program
    {
        static IEnumerable<string> EndlessIncommingStream()
        {
            var random = new Random();
            var strings = new List<string> { "Hey I'm here", "A very looooooooooooooooooooooooooooooooooo o ooo o ooo o ooooo oo ooo o oo oooo ong line", "short one" };
            while(true)
            {
                yield return strings[random.Next(strings.Count)];
                Thread.Sleep(random.Next(100));
            }
        }

        static void Main(string[] args)
        {
            var incommingLogLines = EndlessIncommingStream().ToObservable();

            ILog _logger = LogManager.GetLogger(typeof(Program));

            incommingLogLines.Subscribe(_ => _logger.Info(_));

            Console.ReadKey();
        }
    }
}
