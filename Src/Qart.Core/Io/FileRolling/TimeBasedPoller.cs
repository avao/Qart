using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class TimeBasedPoller
    {
        private Action<byte[], int, int> _processContent;
        private RollingFileReader _rollingFileReader;
        private IObservable<long> _observable;

        public TimeBasedPoller(RollingFileReader fileReader, Action<byte[], int, int> processContent)
        {
            _rollingFileReader = fileReader;
            _processContent = processContent;
            _observable = Observable.Interval(TimeSpan.FromSeconds(10));
            _observable.Subscribe(_ => ElapsedEventHandler());
        }

        private void ElapsedEventHandler()
        {
            const int bufSize = 1000;
            byte[] buf = new byte[bufSize];
            var length = _rollingFileReader.Read(buf, 0, bufSize);
            while(length > 0)
            {
                _processContent(buf, 0, length);
                _rollingFileReader.Ack();
                length = _rollingFileReader.Read(buf, 0, bufSize);
            }
        }
    }
}
