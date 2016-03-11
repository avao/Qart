using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qart.Core.Logging
{
    public interface IDisposableLogger : ILog, IDisposable
    {
    }
}
