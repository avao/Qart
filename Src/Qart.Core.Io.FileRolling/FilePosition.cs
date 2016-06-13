using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class FilePosition
    {
        public long Position { get; private set; }
        public FileId FileId { get; private set; }

        public FilePosition(FileId fileId, long position)
        {
            Position = position;
            FileId = fileId;
        }

        public void UpdatePosition(long pos)
        {
            Position = pos;
        }
    }

    public class FilePositionSerialiser
    {
        public static FilePosition Read(string baseFileName, StreamReader reader)
        {
            var writeTime = reader.ReadDateTime();
            reader.Read();//skip space
            var pos = long.Parse(reader.ReadToEnd());
            return new FilePosition(new FileId(baseFileName, writeTime), pos);
        }

        public static void Write(FilePosition pos, TextWriter writer)
        {
            writer.Write(pos.FileId.WriteTime.ToString("MM/dd/yyyyTHH:mm:ss.fff"));
            writer.Write(" ");
            writer.Write(pos.Position);
        }

    }

    public static class TextReaderExtensions
    {
        public static DateTime ReadDateTime(this TextReader reader)
        {
            char[] buf = new char[23];
            reader.Read(buf, 0, 23);
            return DateTime.ParseExact(new string(buf), "dd/MM/yyyyTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }

}
