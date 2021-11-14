using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.Activation;
using Qart.Core.Comparison;
using Qart.Core.Text;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qart.Testing.Framework
{
    public static class TestCaseContextExtensions
    {
        public static async Task ExecuteActionsAsync(this TestCaseContext context, IObjectFactory<IPipelineAction> pipelineActionFactory, IEnumerable<ResolvableItemDescription> actionDescriptions, bool suppressExceptionsTilltheEnd)
        {
            IList<Exception> exceptions = null;
            foreach (var actionDescription in actionDescriptions)
            {
                if (actionDescription.Name.StartsWith("#"))
                {
                    context.Logger.LogDebug("Skipping {0}", actionDescription.Name);
                    continue;
                }

                context.Logger.LogDebug("Creating action {0} with parameters {1}", actionDescription.Name, actionDescription.Parameters.Select(_ => _.Key + ": " + JsonConvert.SerializeObject(_.Value)));
                var action = pipelineActionFactory.Create(actionDescription.Name, actionDescription.Parameters);
                try
                {
                    await action.ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    if (suppressExceptionsTilltheEnd)
                    {
                        context.Logger.LogWarning("An exception occurred {message}", ex.Message);
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }
                        exceptions.Add(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if(action is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            if (exceptions != null)
                throw new AggregateException(exceptions);
        }

        public static Task AssertContentAsync(this TestCaseContext testCaseContext, JToken item, string path)
            => testCaseContext.AssertContentAsync(item.ToIndentedJson(), path);


        public static Task AssertContentAsync(this TestCaseContext testCaseContext, string content, string path)
            => testCaseContext.TestCase.AssertContentAsync(content, path, testCaseContext.Options.IsRebaseline());

        public static Task AssertContentJsonAsync(this TestCaseContext testCaseContext, string content, string path)
            => testCaseContext.AssertContentAsync(JsonConvert.DeserializeObject<JToken>(content), path);
        

        public static async Task AssertContentJsonManyAsync(this TestCaseContext testCaseContext, string content, string dir, Func<JToken, string> itemNameFunc)
        {
            var exceptions = new List<Exception>();
            //TODO check all is present
            var items = JsonConvert.DeserializeObject<IEnumerable<JToken>>(content);
            foreach (var item in items)
            {
                var path = Path.Combine(dir, itemNameFunc(item));

                try
                {
                    await testCaseContext.AssertContentAsync(item, path);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public static IEnumerable<string> GetDiffCategories(this TestCaseContext testCaseContext, JToken actual, JToken expected, string categoriesPath)
        {
            var categories = testCaseContext.TestCase.GetObjectFromJson<Dictionary<string, IReadOnlyCollection<string>>>(categoriesPath);
            return GetDiffCategories(actual, expected, categories);
        }

        public static IEnumerable<string> GetDiffCategories(JToken actualToken, JToken expectedToken, IDictionary<string, IReadOnlyCollection<string>> categories)
        {
            var knownCategories = categories.Where(kvp => !AreEqual(actualToken, expectedToken, kvp.Value)).Select(kvp => kvp.Key);
            //TODO unknown category
            return knownCategories;
        }

        public static string ResolveValue(this TestCaseContext testCaseContext, string value)
        {
            return VariableResolver.Resolve(value, item => testCaseContext.GetRequiredItem(item));
        }

        private static bool AreEqual(JToken actualToken, JToken expectedToken, IReadOnlyCollection<string> jsonPaths)
        {
            return jsonPaths.All(jsonPath => AreEqual(actualToken.SelectTokens(jsonPath), expectedToken.SelectTokens(jsonPath)));
        }

        private static IEqualityComparer<JToken> equalityComparer = new JTokenEqualityComparer();
        private static bool AreEqual(IEnumerable<JToken> actualTokens, IEnumerable<JToken> expectedTokens)
        {
            return actualTokens.IsEqualTo(expectedTokens, equalityComparer);
        }
    }
}
