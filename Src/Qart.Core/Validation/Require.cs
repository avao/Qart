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
            Require.That(() => value != null, "Expected non null value.");
        }

        public static void NotNullOrEmpty(string value)
        {
            Require.That(() => !string.IsNullOrEmpty(value), "Expected non null and not empty value.");
        }

        public static void DoesNotContain(string value, string substring)
        {
            Require.That(() => !value.Contains(substring), () => "Value should not contain substring [" + substring + "]");
        }

        public static void That(Func<bool> predicate, string failMessage)
        {
            Require.That(predicate, () => failMessage);
        }

        public static void That(Func<bool> predicate, Func<string> fail)
        {
            if(!predicate())
            {
                throw new ArgumentException(fail());
            }
        }
    }   
}
