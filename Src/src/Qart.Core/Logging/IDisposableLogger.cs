using Common.Logging;
using System;

namespace Qart.Core.Logging
{
    public interface IDisposableLogger : ILog, IDisposable
    {
    }
}
