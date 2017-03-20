using Qart.Core.Collections;
using Qart.Testing.ActionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Framework
{
    public static class TestCaseContextExtensions
    {
        public static void ExecuteActions<T>(this TestCaseContext c, IPipelineContextFactory<T> pipelineContextFactory, IPipelineActionFactory<T> actionFactory, IEnumerable<ResolvableItemDescription> actionDescriptions)
        {
            var pipelineContext = pipelineContextFactory.CreateContext(c);
            try
            {
                foreach (var actionDescription in actionDescriptions)
                {
                    c.Logger.DebugFormat("Creating action [{0}] with parameters [{1}]", actionDescription.Name, string.Join("\n", actionDescription.Parameters.Select(_ => _.Key + ": " + _.Value)));
                    var action = actionFactory.Get(actionDescription.Name, actionDescription.Parameters);
                    try
                    {
                        action.Execute(c, pipelineContext);
                    }
                    finally
                    {
                        actionFactory.Release(action);
                    }
                }
            }
            finally
            {
                pipelineContextFactory.Release(pipelineContext);
            }
        }        
    }
}
