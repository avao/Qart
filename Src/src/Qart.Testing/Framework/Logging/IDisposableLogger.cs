using Microsoft.Extensions.Logging;
using System;

namespace Qart.Testing.Framework.Logging
{
    public interface IDisposableLogger : ILogger, IDisposable
    {
    }
}
