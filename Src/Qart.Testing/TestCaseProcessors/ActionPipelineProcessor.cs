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
    public interface IPipelineActionFactory<T>
    {
        IPipelineAction<T> Create(IDictionary<string, object> arguments);
        void Release(IPipelineAction<T> action);
    }

    public class ActionPipelineProcessor<T> : ITestCaseProcessor
    {
        private readonly IPipelineActionFactory<T> _actionFactory;
        private readonly T _pipelineActionContext;
        private readonly IEnumerable<object> _actionDefinitions;

        public ActionPipelineProcessor(T context, IPipelineActionFactory<T> actionFactory, IEnumerable<object> actions)
        {
            _pipelineActionContext = context;
            _actionFactory = actionFactory;
            _actionDefinitions = actions;
        }

        public void Process(TestCaseContext c)
        {
            foreach (var actionDefinition in _actionDefinitions)
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
                action.Execute(new TestCaseContext(c.TestSession, c.TestCase, c.Logger, actionDescriptionWriter), _pipelineActionContext);
            }
        }
    }
}
