using Microsoft.Extensions.Logging;
using Qart.Core.Disposable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Framework.Logging
{
    public class CompositeLogger : IDisposableLogger
    {
        public class LoggerInfo
        {
            public ILogger Logger { get; }
            public bool Dispose { get; }
            public LoggerInfo(ILogger logger, bool dispose)
            {
                Logger = logger;
                Dispose = dispose;
            }
        }

        private readonly LoggerInfo[] _loggerInfos;

        public CompositeLogger(LoggerInfo[] loggerInfos)
        {
            _loggerInfos = loggerInfos;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var disposables = new List<IDisposable>();
            for (int i = 0; i < _loggerInfos.Length; i++)
            {
                disposables.Add(_loggerInfos[i].Logger.BeginScope(state));
            }
            return new CompositeDisposable(disposables);
        }

        public void Dispose()
        {
            for (int i = 0; i < _loggerInfos.Length; i++)
            {
                var info = _loggerInfos[i];
                if (info.Dispose && info.Logger is IDisposable disposableLogger)
                    disposableLogger.Dispose();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _loggerInfos.Any(logger => logger.Logger.IsEnabled(logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            for (int i = 0; i < _loggerInfos.Length; i++)
            {
                _loggerInfos[i].Logger.Log<TState>(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}
