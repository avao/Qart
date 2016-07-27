using Common.Logging;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qart.Testing
{
    public interface IPipelineAction<T>
    {
        string Id { get; }
        void Execute(TestCaseContext testCaseContext, T context);
    }
}
