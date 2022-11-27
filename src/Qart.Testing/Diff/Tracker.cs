using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public class Tracker
    {
        private readonly ITokenSelectorProvider _idProvider;
        private readonly bool _treatNullValueAsMissing;

        private JToken _state;

        public Tracker(ITokenSelectorProvider idProvider, bool treatNullValueAsMissing)
        {
            _idProvider = idProvider;
            _treatNullValueAsMissing = treatNullValueAsMissing;
        }

        public IEnumerable<DiffItem> Push(JToken state)
        {
            var diffs = JsonPatchCreator.Compare(_state, state, _idProvider, _treatNullValueAsMissing);
            _state = state;
            return diffs;
        }
    }
}
