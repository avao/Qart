using NUnit.Framework;
using Qart.Core.Collections;
using System.Linq;

namespace Qart.Core.Tests.Collections
{
    [TestFixture]
    class EnumerableExtensionsTests
    {
        [TestCase(new[] { "a", "b", "c" }, new[] { "a", "b", "c" }, ExpectedResult = new object[] { new[] { "a", "a" }, new[] { "b", "b" }, new[] { "c", "c" } })]
        [TestCase(new[] { "a", "b", "c" }, new[] { "b", "a", "c" }, ExpectedResult = new object[] { new[] { "a", "a" }, new[] { "b", "b" }, new[] { "c", "c" } })]
        [TestCase(new[] { "b", "c" }, new[] { "b", "a", "c" }, ExpectedResult = new object[] { new[] { null, "a" }, new[] { "b", "b" }, new[] { "c", "c" } })]
        [TestCase(new[] { "a", "c" }, new[] { "b", "d", "c" }, ExpectedResult = new object[] { new[] { "a", null }, new[] { null, "b" }, new[] { "c", "c" }, new[] { null, "d" } })]
        [TestCase(new string[] { }, new[] { "b", "d", "c" }, ExpectedResult = new object[] { new[] { null, "b" }, new[] { null, "c" }, new[] { null, "d" } })]
        public object[] EqualEnumerables(object[] lhs, object[] rhs)
        {
            return lhs.OrderBy(_ => _).JoinWithNulls(rhs.OrderBy(_ => _)).Select(kvp => new[] { kvp.Item1, kvp.Item2 }).ToArray();
        }
    }
}
