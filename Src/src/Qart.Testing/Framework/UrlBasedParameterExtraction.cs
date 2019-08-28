using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qart.Testing.Framework
{
    public static class UrlBasedParameterExtraction
    {
        public static ResolvableItemDescription Parse(string definition)
        {
            string actionName = definition;
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            int index = definition.IndexOf('?');
            if (index != -1)
            {
                actionName = definition.Substring(0, index);
                string stringParameters = (index < definition.Length - 1) ? definition.Substring(index + 1) : string.Empty;
                var parametersAsNVC = HttpUtility.ParseQueryString(stringParameters);
                parameters = parametersAsNVC.AllKeys.ToDictionary(_ => _, _ => (object)parametersAsNVC[_]);
            }
            return new ResolvableItemDescription(actionName, parameters);
        }
    }
}
