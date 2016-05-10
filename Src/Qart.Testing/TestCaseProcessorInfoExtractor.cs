using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Newtonsoft.Json.Linq;

namespace Qart.Testing
{
    public class TestCaseProcessorInfoExtractor : ITestCaseProcessorInfoExtractor
    {
        public TestCaseProcessorInfo Execute(TestCase testCase)
        {
            var content = testCase.GetContent(".test").TrimStart();

            var processorId = content.SubstringWhile(_ => char.IsLetterOrDigit(_) || _ == '_');
            var paramContent = content.Substring(processorId.Length).Trim();

            Dictionary<string, object> parameters = null;
            if (!string.IsNullOrEmpty(paramContent))
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramContent);
                parameters = parameters.ToDictionary(_ => _.Key, _ => PostProcess(_.Value));
            }

            return new TestCaseProcessorInfo(processorId, parameters);
        }

        private object PostProcess(object obj)
        {
            var jarray = obj as JArray;
            if (jarray != null)
            {
                switch( GetTokenType(jarray))
                {
                    case JTokenType.String:
                        return jarray.Select(_ => _.Value<string>()).ToArray();
                    case JTokenType.Integer:
                        return jarray.Select(_ => _.Value<int>()).ToArray();
                    case JTokenType.Float:
                        return jarray.Select(_ => _.Value<double>()).ToArray();
                    default:
                        return jarray.ToArray<object>();
                }
            }
            return obj;
        }

        private JTokenType GetTokenType(JArray jarray)
        {
            var distinctTypes = jarray.Select(_ => _.Type).Distinct().ToList();
            if (distinctTypes.Count == 1)
                return distinctTypes[0];

            return JTokenType.Object;
        }
    }
}
