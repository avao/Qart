using System;

namespace Qart.Core.Tracing
{
    public static class Correlation
    {
        public static string GetTickBasedId()
        {
            var bytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            return Convert.ToBase64String(bytes, 0, 6); //getting rid of top 2 bytes
        }
    }
}
