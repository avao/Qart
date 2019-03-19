using Newtonsoft.Json.Linq;
using Qart.Core.Validation;
using System.Collections.Generic;

namespace Qart.Testing.Diff
{
    public class JsonPathPropertyBasedIdProvider : IIdProvider
    {
        private readonly string _propertyName;

        public JsonPathPropertyBasedIdProvider(string propertyName)
        {
            Require.NotNullOrEmpty(propertyName);

            _propertyName = propertyName;
        }

        public string GetId(IEnumerable<string> path, JToken token, int index)
        {
            if (token is JObject jobj)
            {
                var idToken = jobj.GetValue(_propertyName);
                if (idToken != null)
                {
                    return $"[?(@.{_propertyName}=='{idToken.ToString()}')]";
                }
            }
            return $"[{index.ToString()}]";
        }
    }
}
