using NUnit.Framework;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions.Item;
using Qart.Testing.Framework;

namespace Qart.Testing.Tests.ActionPipeline.Actions.Item
{
    class SetItemTests
    {
        [Test]
        public void ResolvesToken()
        {
            var context = new TestCaseContext(null, null, null, new XDocumentDescriptionWriter(null), new ItemsHolder(null));

            context.SetItem("v", "a value");
            var key = context.GetItemKey();
            context.SetItem(key, "1");

            var action1 = new SetItemAction("some text ${v}.");
            action1.Execute(context);

            Assert.That(context.GetItem<string>(key), Is.EqualTo("some text a value."));
        }
    }
}
