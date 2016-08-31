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
        T CreateContext(TestCaseContext c);
        void Release(T context);
    }

    public interface IPipelineActionFactory<T>
    {
        IPipelineAction<T> Create(IDictionary<string, object> arguments);
        IPipelineAction<T> Get(string name, IDictionary<string, object> arguments);
        void Release(IPipelineAction<T> action);
    }

    public class ActionPipelineProcessor<T> : ITestCaseProcessor
    {
        private readonly IPipelineActionFactory<T> _actionFactory;
        private readonly IPipelineContextFactory<T> _pipelineContextFactory;
        private readonly IEnumerable<object> _actionDefinitions;

        public ActionPipelineProcessor(IPipelineContextFactory<T> pipelineContextFactory, IPipelineActionFactory<T> actionFactory, IEnumerable<object> actions)
        {
            _pipelineContextFactory = pipelineContextFactory;
            _actionFactory = actionFactory;
            _actionDefinitions = actions;
        }

        public void Process(TestCaseContext c)
        {
            var pipelineContext = _pipelineContextFactory.CreateContext(c);
            try
            {
                foreach (var actionDefinition in _actionDefinitions)
                {
                    IPipelineAction<T> action = null;
                    var stringActionDef = actionDefinition as string;
                    if (stringActionDef != null)
                    {
                        c.Logger.TraceFormat("Parsing action with definition [{0}]", stringActionDef);
                        var info = UrlBasedParameterExtraction.Parse(stringActionDef);
                        
                        c.Logger.DebugFormat("Creating action [{0}] with parameters [{1}]", info.Name, string.Join("\n", info.Parameters.Select(_ => _.Key + ": " + _.Value)));
                        action = _actionFactory.Get(info.Name, info.Parameters);
                    }
                    else
                    {
                        //TODO implement non-url based parameters
                    }

                    if (action == null)
                    {
                        throw new ArgumentException(string.Format("Unable to resolve pipeline action [{0}]", stringActionDef));
                    }

                    try
                    {
                        action.Execute(c, pipelineContext);
                    }
                    finally
                    {
                        _actionFactory.Release(action);
                    }
                }
            }
            finally
            {
                _pipelineContextFactory.Release(pipelineContext);
            }
        }
    }
}
