using NUnit.Framework;
using Qart.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Tests.Validation
{
    class RequireTests
    {
        [Test]
        public void NotNullFailsWithNull()
        {
            Assert.Throws<ArgumentException>(() => Require.NotNull((string)null));
        }

        [Test]
        public void NotNullSucceedsWithNotNull()
        {
            Require.NotNull("");
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
