using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions.Json;
using Qart.Testing.Context;
using Qart.Testing.Framework;

namespace Qart.Testing.Tests.ActionPipeline.Actions.Item
{
    class JsonEditActionTests
    {
        [Test]
        public void JsonEditAction_ResolvesToken()
        {
            var context = new TestCaseContext(null, null, null, null, new XDocumentDescriptionWriter(null), new ItemsHolder());

            context.SetItem("v", "a value");
            var key = context.GetItemKey();
            context.SetItem(key, "{'abc':1}");

            var action1 = new JsonEditAction("$.abc", "'some text ${v}.'");
            action1.Execute(context);

            Assert.That(context.GetItem<JToken>(key), Is.EqualTo(JValue.Parse("{'abc':'some text a value.'}")));
        }
    }
}
