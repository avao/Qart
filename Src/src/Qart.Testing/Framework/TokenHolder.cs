using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class TokenHolder : ITokenHolder
    {
        private readonly IDictionary<string, IDictionary<string, int>> _tokenGroups;

        public TokenHolder(IDictionary<string, IDictionary<string, int>> tokenGroups)
        {
            _tokenGroups = tokenGroups;
        }

        public string GetName(string group, string path, string value)
        {
            if (!_tokenGroups.TryGetValue(group, out IDictionary<string, int> tokens))
            {
                tokens = new Dictionary<string, int>();
                _tokenGroups.Add(group, tokens);
            }


            if (!tokens.TryGetValue(value, out var index))
            {
                index = tokens.Count + 1;
                tokens.Add(value, index);
            }

            return $"{group}_{index}";
        }
    }
}
