using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qart.Core.DataStore;
using System.Xml.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.TestCaseProcessors
{
    public interface IPipelineContextFactory<T>
    {
        T CreateContext();
    }

    public class ActionPipelineProcessor<T> : ITestCaseProcessor
    {
        private readonly IPipelineContextFactory<T> _contextFactory;
        private readonly IEnumerable<IPipelineAction<T>> _actions;
        private readonly T _pipelineActionContext;

        public ActionPipelineProcessor(IPipelineContextFactory<T> contextFactory, IEnumerable<IPipelineAction<T>> actions)
        {
            _contextFactory = contextFactory;
            _actions = actions;
        }

        public ActionPipelineProcessor(T context, IEnumerable<IPipelineAction<T>> actions)
        {
            _pipelineActionContext = context;
            _actions = actions;
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
                    action = _actions.FirstOrDefault(_ => _.Id == stringActionDef);
                }
                else
                {
                    //TODO
                }

                if (action == null)
                {
                    throw new ArgumentException("Unable to resolve pipeline action");//TODO add context
                }

                var actionDescriptionWriter = c.DescriptionWriter.CreateNestedWriter("action");
                action.Execute(new TestCaseContext(c.TestSession, c.TestCase, c.Logger, actionDescriptionWriter), pipelineActionContext);
            }
        }
    }
}
