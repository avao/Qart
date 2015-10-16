using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qart.Core.Io
{
    public static class StreamExtensions
    {
        public static void UsingStreamReader(this Stream stream, Action<StreamReader> action)
        {
            stream.UsingStreamReader(reader => { action(reader); return true; });
        }

        public static T UsingStreamReader<T>(this Stream stream, Func<StreamReader,T> action)
        {
            using(var reader = new StreamReader(stream))
            {
                return action(reader);
            }
        }
    }
}
