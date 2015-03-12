using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qart.Core.Io.FileRolling;
using System.Threading;
using Qart.Core.Tests.Io;
using Qart.Core.Tests.Io.FileRolling;
using Qart.Core.Io;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var watcher = new FileWatcher(@"c:\work\*\*.txt", _ => Console.WriteLine(_));

            var manager = new RollingFileReaderManager(@"C:\Work\Projects\QartStage\Src\Qart.RandomLogger\bin\Debug\log.txt",
                                                        new FileBasedPositionStore(@"c:\work\output"), 
                                                        ReadBehaviour.FromWhereLeft,
                                                        new DummyOutputProvider(@"c:\work\output"));
            
            //var tests = new FileWatcherTests();
            //tests.NotAPattern();
            //var tests = new RollingFileReaderTests();
            //tests.RolledFilesNoPosition();

            //tests.RolledFilesWithPosition();

            
            Console.ReadKey();
        }
    }
}
