using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Testing.Diff;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Tests.Diff
{
    public class JsonPatchCreatorTests
    {
        private static readonly ITokenSelectorProvider _propertyBasedIdProvider = new PropertyBasedTokenSelectorProvider("Id");

        private static IEnumerable<object[]> CompareTestSource()
        {
            yield return new object[] { new JArray("a", "b"), new JArray("c", "b"), "[{\"JsonPath\":\"$[0]\",\"Value\":\"c\"}]" };
            yield return new object[] { new JArray("a", "b"), new JArray("a"), "[{\"JsonPath\":\"$[1]\",\"Value\":null}]" };
            yield return new object[] { new JArray("a", "b"), new JArray(), "[{\"JsonPath\":\"$[0]\",\"Value\":null},{\"JsonPath\":\"$[1]\",\"Value\":null}]" };
        }

        [TestCaseSource(nameof(CompareTestSource))]
        public void CompareTest(JToken lhs, JToken rhs, string expectedResult)
        {
            var result = JsonPatchCreator.Compare(lhs, rhs, _propertyBasedIdProvider).ToList();
            Assert.That(JsonConvert.SerializeObject(result), Is.EqualTo(expectedResult));
            lhs.ApplyPatch(result);
            Assert.That(lhs, Is.EqualTo(rhs));
        }
    }
}
