using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Testing.ActionPipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing.Framework
{
    public static class TestCaseContextExtensions
    {
        public static void ExecuteActions<T>(this TestCaseContext c, IPipelineContextFactory<T> pipelineContextFactory, IPipelineActionFactory<T> actionFactory, IEnumerable<ResolvableItemDescription> actionDescriptions, bool suppressExceptionsTilltheEnd)
        {
            var pipelineContext = pipelineContextFactory.CreateContext(c);
            try
            {
                IList<Exception> exceptions = null;
                foreach (var actionDescription in actionDescriptions)
                {
                    c.Logger.DebugFormat("Creating action [{0}] with parameters [{1}]", actionDescription.Name, string.Join("\n", actionDescription.Parameters.Select(_ => _.Key + ": " + _.Value)));
                    var action = actionFactory.Get(actionDescription.Name, actionDescription.Parameters);
                    try
                    {
                        action.Execute(c, pipelineContext);
                    }
                    catch (Exception ex)
                    {
                        if(suppressExceptionsTilltheEnd)
                        {
                            if(exceptions == null)
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
            finally
            {
                pipelineContextFactory.Release(pipelineContext);
            }
        }


        public static void AssertContent(this TestCaseContext testCaseContext, JToken item, string path)
        {
            testCaseContext.AssertContent(SerialiseIndented(item), path);
        }

        public static void AssertContent(this TestCaseContext testCaseContext, string content, string path)
        {
            testCaseContext.TestCase.AssertContent(content, path, testCaseContext.Options.IsRebaseline());
        }

        public static void AssertContentJson(this TestCaseContext testCaseContext, string content, string path)
        {
            testCaseContext.AssertContent(JsonConvert.DeserializeObject<JToken>(content), path);
        }

        private static string SerialiseIndented(object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings { Formatting = Formatting.Indented });
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
    }
}
