using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public interface IDiffReporter
    {
        void OnAdded(IEnumerable<string> path, JToken token);
        void OnRemoved(IEnumerable<string> path, JToken token);
        void OnChanged(IEnumerable<string> path, JToken lhs, JToken rhs);
    }
}
