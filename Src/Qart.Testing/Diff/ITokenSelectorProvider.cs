using Newtonsoft.Json.Linq;

namespace Qart.Testing.Diff
{
    public interface ITokenSelectorProvider
    {
        string GetTokenSelector(string jsonPath, JToken token, int index);
    }
}
