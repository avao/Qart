using Newtonsoft.Json.Linq;

namespace Qart.Testing.Diff
{
    public record class DiffItem (string JsonPath, JToken Value);
}
