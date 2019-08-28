using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Logging
{
    public class CompositeLogger : IDisposableLogger
    {
        public class LoggerInfo
        {
            public ILog Logger { get; }
            public bool Reusable { get; }

            public LoggerInfo(ILog logger, bool reusable)
            {
                Logger = logger;
                Reusable = reusable;
            }
        }

        private readonly IEnumerable<ILog> _loggers;
        private readonly IEnumerable<LoggerInfo> _loggerInfos;

        public CompositeLogger(IEnumerable<LoggerInfo> loggerInfos)
        {
            _loggerInfos = loggerInfos;
            _loggers = _loggerInfos.Select(_ => _.Logger).ToList();
        }

        public bool IsDebugEnabled => _loggers.Any(_ => _.IsDebugEnabled);
        public bool IsErrorEnabled => _loggers.Any(_ => _.IsErrorEnabled);
        public bool IsFatalEnabled => _loggers.Any(_ => _.IsFatalEnabled);
        public bool IsInfoEnabled => _loggers.Any(_ => _.IsInfoEnabled);
        public bool IsTraceEnabled => _loggers.Any(_ => _.IsTraceEnabled);
        public bool IsWarnEnabled => _loggers.Any(_ => _.IsWarnEnabled);
        public IVariablesContext GlobalVariablesContext => _loggers.First().GlobalVariablesContext;
        public IVariablesContext ThreadVariablesContext => _loggers.First().ThreadVariablesContext;
        public INestedVariablesContext NestedThreadVariablesContext => _loggers.First().NestedThreadVariablesContext;

        public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(formatProvider, formatMessageCallback, exception);
            }
        }

        public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(formatProvider, formatMessageCallback);
            }
        }

        public void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(formatMessageCallback, exception);
            }
        }

        public void Debug(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(formatMessageCallback);
            }
        }

        public void Debug(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(message, exception);
            }
        }

        public void Debug(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(message);
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.DebugFormat(formatProvider, format, exception, args);
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.DebugFormat(formatProvider, format, args);
            }
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.DebugFormat(format, exception, args);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.DebugFormat(format, args);
            }
        }

        public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(formatProvider, formatMessageCallback, exception);
            }
        }

        public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(formatProvider, formatMessageCallback);
            }
        }

        public void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(formatMessageCallback, exception);
            }
        }

        public void Error(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(formatMessageCallback);
            }
        }

        public void Error(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message, exception);
            }
        }

        public void Error(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message);
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.ErrorFormat(formatProvider, format, exception, args);
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.ErrorFormat(formatProvider, format, args);
            }
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.ErrorFormat(format, exception, args);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.ErrorFormat(format, args);
            }
        }

        public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(formatProvider, exception);
            }
        }

        public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(formatProvider, formatMessageCallback);
            }
        }

        public void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(formatMessageCallback, exception);
            }
        }

        public void Fatal(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(formatMessageCallback);
            }
        }

        public void Fatal(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(message, exception);
            }
        }

        public void Fatal(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(message);
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.FatalFormat(formatProvider, format, exception, args);
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.FatalFormat(formatProvider, format, args);
            }
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.FatalFormat(format, exception, args);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.FatalFormat(format, args);
            }
        }



        public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(formatProvider, formatMessageCallback, exception);
            }
        }

        public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(formatProvider, formatMessageCallback);
            }
        }

        public void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(formatMessageCallback, exception);
            }
        }

        public void Info(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(formatMessageCallback);
            }
        }

        public void Info(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message, exception);
            }
        }

        public void Info(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message);
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.InfoFormat(formatProvider, format, exception, args);
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.InfoFormat(formatProvider, format, args);
            }
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.InfoFormat(format, exception, args);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.InfoFormat(format, args);
            }
        }


        public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(formatProvider, formatMessageCallback, exception);
            }
        }

        public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(formatProvider, formatMessageCallback);
            }
        }

        public void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(formatMessageCallback, exception);
            }
        }

        public void Trace(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(formatMessageCallback);
            }
        }

        public void Trace(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(message, exception);
            }
        }

        public void Trace(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Trace(message);
            }
        }

        public void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.TraceFormat(formatProvider, format, exception, args);
            }
        }

        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.TraceFormat(formatProvider, format, args);
            }
        }

        public void TraceFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.TraceFormat(format, exception, args);
            }
        }

        public void TraceFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.TraceFormat(format, args);
            }
        }

        public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(formatProvider, formatMessageCallback, exception);
            }
        }

        public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(formatProvider, formatMessageCallback);
            }
        }

        public void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(formatMessageCallback, exception);
            }
        }

        public void Warn(Action<FormatMessageHandler> formatMessageCallback)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(formatMessageCallback);
            }
        }

        public void Warn(object message, Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(message, exception);
            }
        }

        public void Warn(object message)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(message);
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.WarnFormat(formatProvider, format, exception, args);
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.WarnFormat(formatProvider, format, args);
            }
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.WarnFormat(format, exception, args);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.WarnFormat(format, args);
            }
        }

        public void Dispose()
        {
            foreach (var logger in _loggerInfos.Where(_ => !_.Reusable).Select(_ => _.Logger))
            {
                ((IDisposable)logger).Dispose();
            }
        }
    }
}
