using Common.Logging;
using Common.Logging.Simple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qart.Core.Logging
{
    public class TextWriterLogger : AbstractSimpleLogger, IDisposableLogger
    {
        TextWriter _writer;

        public TextWriterLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat, TextWriter writer)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
            _writer = writer;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            FormatOutput(sb, level, message, exception);
            _writer.WriteLine(sb.ToString());
        }

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }
    }
}
