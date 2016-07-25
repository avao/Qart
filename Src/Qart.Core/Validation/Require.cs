using System;

namespace Qart.Core.Validation
{
    public static class Require
    {
        public static T NotNull<T>(T value)
            where T : class
        {
            return NotNull(value, "Expected non null value.");
        }

        public static T NotNull<T>(T value, string message)
            where T : class
        {
            Require.That(() => value != null, message);
            return value;
        }

        public static string NotNullOrEmpty(string value)
        {
            Require.NotNullOrEmpty(value, "Expected non null and not empty value.");
            return value;
        }

        public static void NotNullOrEmpty(string value, string message)
        {
            Require.That(() => !string.IsNullOrEmpty(value), message);
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
