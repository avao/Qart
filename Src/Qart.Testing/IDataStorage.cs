using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface IDataStorage
    {
        Stream GetStream(string testCaseId, string itemId);
        string GetContent(string testCaseId, string itemId);

        void PutContent(string testCaseId, string itemId, string content);
    }
}
