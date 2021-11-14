using System;
using System.Diagnostics;

namespace Qart.Core.Timers
{
    public class DisposableTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Action<long> _onCompletion;

        public DisposableTimer(Action<long> onCompletion)
        {
            _stopwatch = Stopwatch.StartNew();
            _onCompletion = onCompletion;
        }

        public void Dispose()
        {
            _onCompletion(_stopwatch.ElapsedMilliseconds);
        }
    }

}
