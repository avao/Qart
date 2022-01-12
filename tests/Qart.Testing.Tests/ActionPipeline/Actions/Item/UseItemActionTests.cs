using NUnit.Framework;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions.Item;
using Qart.Testing.Framework;
using System.Threading.Tasks;

namespace Qart.Testing.Tests.ActionPipeline.Actions.Item
{
    class UseItemActionTests
    {
        [Test]
        public async Task UseItemAction_SetsItemKey()
        {
            var context = new TestCaseContext(null, null, null, null, new XDocumentDescriptionWriter(null), new ItemsHolder());

            Assert.That(context.GetItemKey(), Is.EqualTo(string.Empty));

            const string aKey1 = "AKey1";
            var action1 = new UseItemKeyAction(aKey1);
            await action1.ExecuteAsync(context);

            Assert.That(context.GetItemKey(), Is.EqualTo(aKey1));

            const string aKey2 = "AKey2";
            var action2 = new UseItemKeyAction(aKey2);
            await action2.ExecuteAsync(context);

            Assert.That(context.GetItemKey(), Is.EqualTo(aKey2));
        }
    }
}
