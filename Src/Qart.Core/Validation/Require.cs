using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Validation
{
    public static class Require
    {
        public static void NotNull<T>(T value)
            where T : class
        {
            if (value == null)
                throw new ArgumentException("Expected non null value.");
        }
    }
}
