using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public class DiffItem
    {
        public IEnumerable<string> Path { get; }
        public JToken Lhs { get; }
        public JToken Rhs { get; }

        public DiffItem(IEnumerable<string> path, JToken lhs, JToken rhs)
        {
            Path = path;
            Lhs = lhs;
            Rhs = rhs;
        }

        public bool IsInsert() => Lhs == null && Rhs != null;
        public bool IsRemoval() => Lhs != null && Rhs == null;
        public bool IsUpdate() => Lhs != null && Rhs != null;
    }
}
