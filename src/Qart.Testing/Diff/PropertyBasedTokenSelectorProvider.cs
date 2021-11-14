using Newtonsoft.Json.Linq;
using Qart.Core.Validation;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.Diff
{
    public class PropertyBasedTokenSelectorProvider : ITokenSelectorProvider
    {
        private readonly string _propertyName;

        public PropertyBasedTokenSelectorProvider(string propertyName)
        {
            Require.NotNullOrEmpty(propertyName);

            _propertyName = propertyName;
        }

        public string GetTokenSelector(string jsonPath, JToken token, int index)
        {
            if (token is JObject jobj)
            {
                var idToken = jobj.GetValue(_propertyName);
                if (idToken != null)
                {
                    return JsonPathFormatter.FormatAndCondition(new[] { (_propertyName, idToken) });
                }
            }
            return JsonPathFormatter.FormatIndex(index);
        }
    }
}
