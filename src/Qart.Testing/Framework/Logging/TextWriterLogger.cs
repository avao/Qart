using Microsoft.Extensions.Logging;
using Qart.Core.Disposable;
using System;
using System.IO;

namespace Qart.Testing.Framework.Logging
{
    public class TextWriterLogger : IDisposableLogger
    {
        private readonly LogLevel _logLevel;
        private readonly TextWriter _textWriter;

        public TextWriterLogger(LogLevel logLevel, TextWriter textWriter)
        {
            _logLevel = logLevel;
            _textWriter = textWriter;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public void Dispose()
        {
            _textWriter.Dispose();
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [{ logLevel }] : {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }

            _textWriter.WriteLine(message);
        }
    }
}
