using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestSystem
    {
        public IDataStorage DataStorage { get; private set; }

        public TestSystem(IDataStorage dataStorage)
        {
            DataStorage = dataStorage;
        }

        public TestCase GetTestCase(string id)
        {
            return new TestCase(id, this);
        }
    }
}
