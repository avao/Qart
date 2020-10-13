using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline
{
    public class UrlBasedActionPipelineProcessor : IActionPipelineProcessor
    {
        private readonly IPipelineActionFactory _actionFactory;

        public IEnumerable<ResolvableItemDescription> ActionDecsriptions { get; private set; }

        public UrlBasedActionPipelineProcessor(IPipelineActionFactory actionFactory, IEnumerable<object> actions)
        {
            _actionFactory = actionFactory;
            ActionDecsriptions = actions.Select(Convert).ToList();
        }

        public void Process(TestCaseContext c)
        {
            c.ExecuteActions(_actionFactory, ActionDecsriptions, c.Options.GetDeferExceptions());
        }

        private static ResolvableItemDescription Convert(object actionDescription)
        {
            if (actionDescription is string stringActionDef)
            {
                return UrlBasedParameterExtraction.Parse(stringActionDef.AsSpan());
            }

            if (actionDescription is IDictionary<string, object> dictionaryActionDef)
            {
                return new ResolvableItemDescription((string)dictionaryActionDef["id"], dictionaryActionDef);
            }

            throw new Exception(string.Format("Could not convert action definition to url [{0}]", actionDescription));
        }
    }
}
