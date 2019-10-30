using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.Collections;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Diff;
using Qart.Testing.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing.Framework
{
    public static class TestCaseContextExtensions
    {
        public static void ExecuteActions(this TestCaseContext context, IPipelineActionFactory actionFactory, IEnumerable<ResolvableItemDescription> actionDescriptions, bool suppressExceptionsTilltheEnd)
        {
            IList<Exception> exceptions = null;
            foreach (var actionDescription in actionDescriptions)
            {
                context.Logger.LogDebug("Creating action {0} with parameters {1}", actionDescription.Name, actionDescription.Parameters.Select(_ => _.Key + ": " + JsonConvert.SerializeObject(_.Value)));
                var action = actionFactory.Get(actionDescription.Name, actionDescription.Parameters);
                try
                {
                    action.Execute(context);
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
                    actionFactory.Release(action);
                }
            }

            if (exceptions != null)
                throw new AggregateException(exceptions);
        }

        public static void AssertContent(this TestCaseContext testCaseContext, JToken item, string path)
        {
            testCaseContext.AssertContent(item.ToIndentedJson(), path);
        }

        public static void AssertContent(this TestCaseContext testCaseContext, string content, string path)
        {
            testCaseContext.TestCase.AssertContent(content, path, testCaseContext.Options.IsRebaseline());
        }

        public static void AssertContentJson(this TestCaseContext testCaseContext, string content, string path)
        {
            testCaseContext.AssertContent(JsonConvert.DeserializeObject<JToken>(content), path);
        }

        public static void AssertContentJsonMany(this TestCaseContext testCaseContext, string content, string dir, Func<JToken, string> itemNameFunc)
        {
            var exceptions = new List<Exception>();
            //TODO check all is present
            var items = JsonConvert.DeserializeObject<IEnumerable<JToken>>(content);
            foreach (var item in items)
            {
                var path = Path.Combine(dir, itemNameFunc(item));

                try
                {
                    testCaseContext.AssertContent(item, path);
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
