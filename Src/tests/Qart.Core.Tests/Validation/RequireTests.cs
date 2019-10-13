using NUnit.Framework;
using Qart.Core.Validation;
using System;

namespace Qart.Core.Tests.Validation
{
    class RequireTests
    {
        [Test]
        public void NotNullFailsWithNull()
        {
            Assert.Throws<ArgumentException>(() => Require.NotNull((string)null, "message"));
        }

        [Test]
        public void NotNullSucceedsWithNotNull()
        {
            Require.NotNull("", "message");
        }

        [Test]
        public void DoesNotContainFails()
        {
            Assert.Throws<ArgumentException>(() => Require.DoesNotContain("abcde", "e"));
        }

        [Test]
        public void DoesNotContainSucceeds()
        {
            Require.DoesNotContain("abcde", "?");
        }
    }
}
