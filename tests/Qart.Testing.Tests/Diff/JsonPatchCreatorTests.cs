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
            yield return new object[] { new JArray("a", "b"), new JArray("c", "b"), true, "[{\"JsonPath\":\"$[0]\",\"Value\":\"c\"}]" };
            yield return new object[] { new JArray("a", "b"), new JArray("a"), true, "[{\"JsonPath\":\"$[1]\",\"Value\":null}]" };
            yield return new object[] { new JArray("a", "b"), new JArray(), true, "[{\"JsonPath\":\"$[0]\",\"Value\":null},{\"JsonPath\":\"$[1]\",\"Value\":null}]" };
            yield return new object[] { new JArray(), new JArray("a"), true, "[{\"JsonPath\":\"$[0]\",\"Value\":\"a\"}]" };
            yield return new object[] { new JObject(new JProperty("prop", null)), new JObject(), false, "[{\"JsonPath\":\"$.prop\",\"Value\":null}]" };
            yield return new object[] { new JObject(), new JObject(new JProperty("prop", null)), false, "[{\"JsonPath\":\"$.prop\",\"Value\":null}]" };
            yield return new object[] { new JObject(new JProperty("prop", null)), new JObject(), true, "[]" };
            yield return new object[] { new JObject(), new JObject(new JProperty("prop", null)), true, "[]" };
        }

        [TestCaseSource(nameof(CompareTestSource))]
        public void CompareTest(JToken lhs, JToken rhs, bool treatNullValueAsMissing, string expectedResult)
        {
            var result = JsonPatchCreator.Compare(lhs, rhs, _propertyBasedIdProvider, treatNullValueAsMissing).ToList();
            Assert.That(JsonConvert.SerializeObject(result), Is.EqualTo(expectedResult));
            lhs.ApplyPatch(result);
            
            if(!treatNullValueAsMissing)
            {
                Assert.That(lhs, Is.EqualTo(rhs));
            }
            Assert.That(JsonPatchCreator.Compare(lhs, rhs, _propertyBasedIdProvider, treatNullValueAsMissing).ToList(), Is.Empty);
        }
    }
}
