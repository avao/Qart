using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface ITestSession : IDisposable
    {
    }

    public class TestSession : ITestSession
    {
        public void Dispose()
        {
        }
    }
}
