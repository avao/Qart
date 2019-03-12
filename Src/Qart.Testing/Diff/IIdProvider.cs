using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public interface IIdProvider
    {
        string GetId(IEnumerable<string> path, JToken token, int index);
    }
}
