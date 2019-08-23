using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.Tests
{
    public class JsonExtensionTests
    {
        [Test, Ignore("")]
        public void ScopedDataStore()
        {
            var dataStore = new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(@"TestData\JsonExtensions\Order"));
            var jobj = dataStore.GetObjectFromJson<JObject>("array.json");
            jobj.Order("blah", new[] { "" });

            //Assert.AreEqual(new[] { Path.Combine("Ref", "artifact.xml.ref") }, scopedDataStore.GetItemIds("Ref"));
        }
    }
}
