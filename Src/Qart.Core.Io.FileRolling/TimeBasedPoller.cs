using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class TimeBasedPoller : IDisposable
    {
        private Func<byte[], int, int, bool> _processContent;
        private RollingFileReader _rollingFileReader;
        private IObservable<long> _observable;
        private CancellationTokenSource _cancelationTokenSource;

        public TimeBasedPoller(RollingFileReader fileReader, Func<byte[], int, int, bool> processContent)
        {
            _rollingFileReader = fileReader;
            _processContent = processContent;
            _observable = Observable.Interval(TimeSpan.FromSeconds(10));
            _cancelationTokenSource = new CancellationTokenSource();
            _observable.Subscribe(_ => ElapsedEventHandler(), _cancelationTokenSource.Token);
        }

        private void ElapsedEventHandler()
        {
            if (_cancelationTokenSource.IsCancellationRequested)
                return;

            const int bufSize = 1000;
            byte[] buf = new byte[bufSize];
            var length = _rollingFileReader.Read(buf, 0, bufSize);
            while(length > 0)
            {
                if (_processContent(buf, 0, length))
                {
                    _rollingFileReader.Ack();
                }
                else
                {
                    _rollingFileReader.RollBack();
                    break;
                }
                length = _rollingFileReader.Read(buf, 0, bufSize);
            }
        }

        public void Dispose()
        {
            _cancelationTokenSource.Cancel();
        }
    }
}
