﻿using System;

namespace Qart.Core.Validation
{
    public static class Require
    {
        public static void NotNull<T>(T value, string message) where T : class => That(() => value != null, message);

        public static void NotNullOrEmpty(string value) => NotNullOrEmpty(value, "Expected non null and not empty value.");
        public static void NotNullOrEmpty(string value, string message) => That(() => !string.IsNullOrEmpty(value), message);
        public static void DoesNotContain(string value, string substring) => That(() => !value.Contains(substring), () => "Value should not contain substring [" + substring + "]");
        public static void Equal(string lhs, string rhs, string message) => That(() => lhs == rhs, message);
        public static void NotEqual(string lhs, string rhs, string message) => That(() => lhs != rhs, message);

        public static void That(Func<bool> predicate, string failMessage) => That(predicate, () => failMessage);

        public static void That(Func<bool> predicate, Func<string> messageFunc)
        {
            if (!predicate())
            {
                Fail(messageFunc);
            }
        }

        public static void Fail(Func<string> messageFunc) => Fail(messageFunc());
        public static void Fail(string message) => throw new ArgumentException(message);
    }

    public static class RequireExtensions
    {
        public static T RequireNotNull<T>(this T obj, string message)
            where T : class
        {
            Require.NotNull(obj, message);
            return obj;
        }

        public static T RequireType<T>(this object obj, string message)
            where T : class
        {
            if (obj is T value)
            {
                return value;
            }

            Require.Fail(message);
            throw new NotSupportedException("Just to make compiler happy");
        }
    }
}
