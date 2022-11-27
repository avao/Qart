using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Testing.Diff;
using System;
using System.Collections.Generic;

namespace Qart.Testing.Tests.Diff
{
    public class JsonPatchExtensionsTests
    {
        private static readonly ITokenSelectorProvider _propertyBasedIdProvider = new PropertyBasedTokenSelectorProvider("Id");

        private static IEnumerable<object[]> AddTokenTestSource()
        {
            yield return new object[] { JObject.FromObject(new { a = new { prop = "abc" } }), "$.a.newProp", new JValue(1), "{\"a\":{\"prop\":\"abc\",\"newProp\":1}}" };
            yield return new object[] { JObject.FromObject(new { a = new { prop = Array.Empty<string>() } }), "$.a.prop[0]", new JValue(1), "{\"a\":{\"prop\":[1]}}" };
            yield return new object[] { JObject.FromObject(new { a = new { prop = "abc" } }), "$.a.newProp", null, "{\"a\":{\"prop\":\"abc\",\"newProp\":null}}" };
        }

        [TestCaseSource(nameof(AddTokenTestSource))]
        public void AddTokenTest(JToken lhs, string jsonPath, JToken tokenToAdd, string expectedResult)
        {
            lhs.AddTokenByJsonPath(jsonPath, tokenToAdd);
            Assert.That(JsonConvert.SerializeObject(lhs), Is.EqualTo(expectedResult));
        }
    }
}
