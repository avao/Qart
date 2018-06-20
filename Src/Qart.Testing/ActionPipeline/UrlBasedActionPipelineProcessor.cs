using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline
{
    public class UrlBasedActionPipelineProcessor<T> : IActionPipelineProcessor
    {
        private readonly IPipelineActionFactory<T> _actionFactory;
        private readonly IPipelineContextFactory<T> _pipelineContextFactory;
        
        public IEnumerable<ResolvableItemDescription> ActionDecsriptions { get; private set; }

        public UrlBasedActionPipelineProcessor(IPipelineContextFactory<T> pipelineContextFactory, IPipelineActionFactory<T> actionFactory, IEnumerable<object> actions)
        {
            _pipelineContextFactory = pipelineContextFactory;
            _actionFactory = actionFactory;
            ActionDecsriptions = actions.Select(Convert).ToList();
        }

        public void Process(TestCaseContext c)
        {
            c.ExecuteActions(_pipelineContextFactory, _actionFactory, ActionDecsriptions, c.Options.GetDeferExceptions());
        }

        private static ResolvableItemDescription Convert(object actionDescription)
        {
            var stringActionDef = actionDescription as string;
            if (stringActionDef == null)
            {
                throw new Exception(string.Format("Could not convert action definition to url [{0}]", actionDescription));
            }

            return UrlBasedParameterExtraction.Parse(stringActionDef);
        }
    }
}
