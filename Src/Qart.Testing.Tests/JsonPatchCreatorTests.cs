using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Testing.Diff;

namespace Qart.Testing.Tests
{
    [TestFixture]
    class JsonPatchCreatorTests
    {
        private static readonly PropertyBasedIdProvider _propertyBasedIdProvider = new PropertyBasedIdProvider("Id");

        //TODO placeholder
        [Test]
        public void Compare()
        {
            var result = JsonPatchCreator.Compare(new JArray("a", "b"), new JArray("c", "b"), _propertyBasedIdProvider);
        }
    }
}
