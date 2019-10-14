using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.Tests
{
    public class JsonExtensionsTests
    {
        [Test]
        public void OrderSimpleValues()
        {
            var array = new JArray(new JToken[] { "b", "c", "a" });
            array.OrderItems((token) => token.SelectToken("$"));
            Assert.That(array, Is.EqualTo(new JArray(new JToken[] { "a", "b", "c" })));
        }
    }
}
