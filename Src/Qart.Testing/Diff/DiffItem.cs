using Newtonsoft.Json.Linq;

namespace Qart.Testing.Diff
{
    public class DiffItem
    {
        public string JsonPath { get; }
        public JToken Value { get; }

        public DiffItem(string jsonPath, JToken value)
        {
            JsonPath = jsonPath;
            Value = value;
        }
    }
}
