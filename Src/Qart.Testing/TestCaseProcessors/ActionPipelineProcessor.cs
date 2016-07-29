using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Qart.Core.DataStore;
using Qart.Core.Text;
using System.Xml.Linq;
using Qart.Testing.Framework;
using System.Web;

namespace Qart.Testing.TestCaseProcessors
{
    public interface IPipelineContextFactory<T>
    {
        T CreateContext();
    }

    public interface IPipelineActionFactory<T>
    {
        IPipelineAction<T> Create(IDictionary<string, object> arguments);
        void Release(IPipelineAction<T> action);
    }

    public class ActionPipelineProcessor<T> : ITestCaseProcessor
    {
        private readonly IPipelineContextFactory<T> _contextFactory;
        private readonly IPipelineActionFactory<T> _actionFactory;
        private readonly T _pipelineActionContext;

        public ActionPipelineProcessor(IPipelineContextFactory<T> contextFactory, IPipelineActionFactory<T> actionFactory)
        {
            _contextFactory = contextFactory;
            _actionFactory = actionFactory;
        }

        public ActionPipelineProcessor(T context, IPipelineActionFactory<T> actionFactory)
        {
            _pipelineActionContext = context;
            _actionFactory = actionFactory;
        }

        public void Process(TestCaseContext c)
        {
            T pipelineActionContext;
            if (_contextFactory != null)
            {
                pipelineActionContext = _contextFactory.CreateContext();
            }
            else
            {
                pipelineActionContext = _pipelineActionContext;
            }

            string actionsContent = c.TestCase.GetContent("actions");
            var actionDefinitions = JsonConvert.DeserializeObject<List<object>>(actionsContent);
            foreach (var actionDefinition in actionDefinitions)
            {
                IPipelineAction<T> action = null;
                var stringActionDef = actionDefinition as string;
                if (stringActionDef != null)
                {
                    string actionName = stringActionDef;
                    IDictionary<string, object> parameters = new Dictionary<string, object>();

                    int index = stringActionDef.IndexOf('?');
                    if (index != -1)
                    {
                        actionName = stringActionDef.Substring(0, index);
                        string stringParameters = (index < stringActionDef.Length - 1) ? stringActionDef.Substring(index + 1) : String.Empty;
                        var parametersAsNVC = HttpUtility.ParseQueryString(stringParameters);
                        parameters = parametersAsNVC.AllKeys.ToDictionary(_ => _, _ => (object)parametersAsNVC[_]);
                    }

                    action = _actionFactory.Create(parameters);
                }
                else
                {
                    //TODO implement non-url based parameters
                }

                if (action == null)
                {
                    throw new ArgumentException(string.Format("Unable to resolve pipeline action [{0}]", stringActionDef));
                }

                var actionDescriptionWriter = c.DescriptionWriter.CreateNestedWriter("action");
                action.Execute(new TestCaseContext(c.TestSession, c.TestCase, c.Logger, actionDescriptionWriter), pipelineActionContext);
            }
        }
    }
}
