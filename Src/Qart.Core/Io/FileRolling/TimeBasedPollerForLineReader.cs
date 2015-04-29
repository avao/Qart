using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class TimeBasedPollerForLineReader
    {
        private Func<string, bool> _processContent;
        private RollingFileTextReader _rollingFileReader;
        private IObservable<long> _observable;

        public TimeBasedPollerForLineReader(RollingFileReader fileReader, Func<string, bool> processContent)
        {
            _rollingFileReader = new RollingFileTextReader(fileReader);
            _processContent = processContent;
            _observable = Observable.Interval(TimeSpan.FromSeconds(10));
            _observable.Subscribe(_ => ElapsedEventHandler());
        }

        private void ElapsedEventHandler()
        {
            string line = _rollingFileReader.ReadLine();
            while (line != null)
            {
                if (_processContent(line))
                {
                    _rollingFileReader.Ack();
                }
                else
                {
                    _rollingFileReader.RollBack();
                    break;
                }
                line = _rollingFileReader.ReadLine();
            }
        }
    }
}
