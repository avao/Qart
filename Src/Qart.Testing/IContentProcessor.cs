using Qart.Core.DataStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface IContentProcessor
    {
        Stream Process(string content, IDataStore dataStore);
    }
}
