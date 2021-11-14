using System.Collections.Generic;

namespace Qart.Core.Activation
{
    public class ActivationRegistry<T>
    {
        private readonly IDictionary<string, ActivationInfo> _items;

        public IEnumerable<string> Aliases => _items.Keys;


        public ActivationRegistry()
        {
            _items = new Dictionary<string, ActivationInfo>();
        }

        public void Register<TImpl>(string alias, IDictionary<string, object> parameters = null)
            where TImpl : T
            => _items.Add(alias, ActivationInfo.Create<TImpl>(parameters));

        public bool TryGetValue(string alias, out ActivationInfo activationInfo)
            => _items.TryGetValue(alias, out activationInfo);
    }
}
