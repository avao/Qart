using System;

namespace Qart.Core.Disposable
{
    public class NoopDisposable : IDisposable
    {
        public static IDisposable Instance = new NoopDisposable();

        public void Dispose()
        {
        }
    }
}
