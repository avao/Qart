using NUnit.Framework;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions.Item;
using Qart.Testing.Framework;

namespace Qart.Testing.Tests.ActionPipeline.Actions.Item
{
    class UseItemActionTests
    {
        [Test]
        public void UseItemAction_SetsItemKey()
        {
            var context = new TestCaseContext(null, null, null, new XDocumentDescriptionWriter(null), new ItemsHolder(null));

            Assert.That(context.GetItemKey(), Is.EqualTo(string.Empty));

            const string aKey1 = "AKey1";
            var action1 = new UseItemKeyAction(aKey1);
            action1.Execute(context);

            Assert.That(context.GetItemKey(), Is.EqualTo(aKey1));

            const string aKey2 = "AKey2";
            var action2 = new UseItemKeyAction(aKey2);
            action2.Execute(context);

            Assert.That(context.GetItemKey(), Is.EqualTo(aKey2));
        }
    }
}
