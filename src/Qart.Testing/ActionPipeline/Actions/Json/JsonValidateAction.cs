using Json.Schema;
using Microsoft.Extensions.Logging;
using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonValidateAction : IPipelineAction
    {
        private readonly string _schemaPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonValidateAction(string schemaPath, string sourceKey = null, string targetKey = null)
        {
            _schemaPath = schemaPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("JsonValidate", $"{effectiveSourceKey}");
            var itemToken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);

            var schema = await testCaseContext.TestCase.UsingReadStream(_schemaPath, stream => JsonSchema.FromStream(stream));

            var doc = JsonDocument.Parse(itemToken.ToString());

            var validationResult = schema.Validate(doc.RootElement, new ValidationOptions { OutputFormat = OutputFormat.Basic });
            testCaseContext.Logger.LogDebug("Document is valid agains schema: {result}", validationResult.IsValid);

            var res = validationResult.NestedResults.Select(r => new { r.Message, InstanceLocation = r.InstanceLocation.Source, SchemaLocation = r.SchemaLocation.Source }).ToIndentedJson();
            testCaseContext.SetItem(effectiveTargetKey, res);
        }
    }
}
