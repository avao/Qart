using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Newtonsoft.Json.Linq;

namespace Qart.Testing.Framework
{
    public class TestCaseProcessorInfoExtractor 
    {
        public ResolvableItemDescription Execute(TestCase testCase)
        {
            var content = testCase.GetContent(".test").TrimStart();

            string processorId;
            IDictionary<string, object> parameters = null;

            //hacky check for json format
            if (!content.StartsWith("{"))
            {
                processorId = content.SubstringWhile(_ => char.IsLetterOrDigit(_) || _ == '_');
                var paramContent = content.Substring(processorId.Length).Trim();
                if (!string.IsNullOrEmpty(paramContent))
                {
                    parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramContent);
                }
            }
            else
            {
                var parsedJson = JsonConvert.DeserializeObject<ResolvableItemDescription>(content, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                processorId = parsedJson.Name;
                parameters = parsedJson.Parameters;
            }

            return new ResolvableItemDescription(processorId, PostProcess(parameters));
        }

        private IDictionary<string, object> PostProcess(IDictionary<string, object> obj)
        {
            return obj.ToDictionary(_ => _.Key, _ => PostProcess(_.Value));
        }

        private object PostProcess(object obj)
        {
            var jvalue = obj as JValue;
            if(jvalue != null)
            {
                switch( jvalue.Type)
                {
                    case JTokenType.String:
                        return jvalue.Value<string>();
                    case JTokenType.Integer:
                        return jvalue.Value<int>();
                    case JTokenType.Float:
                        return jvalue.Value<double>();
                    default:
                        return jvalue.Value<object>();
                }
            }
            else
            {
                var jarray = obj as JArray;
                if (jarray != null)
                {
                    switch (GetTokenType(jarray))
                    {
                        case JTokenType.String:
                            return jarray.Select(_ => _.Value<string>()).ToArray();
                        case JTokenType.Integer:
                            return jarray.Select(_ => _.Value<int>()).ToArray();
                        case JTokenType.Float:
                            return jarray.Select(_ => _.Value<double>()).ToArray();
                        default:
                            return PostProcess(jarray.ToArray<object>());
                    }
                }
                else
                {
                    var jobj = obj as JObject;
                    if (jobj != null)
                    {
                        return jobj.Properties().ToDictionary(_ => _.Name, _ => PostProcess(_.Value));
                    }
                }
            }
            
            return obj;
        }

        private object[] PostProcess(object[] obj)
        {
            return obj.Select(_ => PostProcess(_)).ToArray();
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
