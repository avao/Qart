using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public class Tracker
    {
        private readonly ITokenSelectorProvider _idProvider;

        private JToken _state;

        public Tracker(ITokenSelectorProvider idProvider)
        {
            _idProvider = idProvider;
        }

        public IEnumerable<DiffItem> Push(JToken state)
        {
            var diffs = JsonPatchCreator.Compare(_state, state, _idProvider);
            _state = state;
            return diffs;
        }
    }
}
