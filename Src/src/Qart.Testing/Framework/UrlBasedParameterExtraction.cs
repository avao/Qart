using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;

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

        public static ResolvableItemDescription Parse(ReadOnlySpan<char> definition)
        {
            int index = definition.IndexOf('?');
            if (index == -1) return new ResolvableItemDescription(string.Empty, new Dictionary<string, object>());

            var actionName = definition.Slice(0, index);
            var stringParameters = (index < definition.Length - 1) 
                ? definition.Slice(index+1) 
                : ReadOnlySpan<char>.Empty;
            var parametersAsNVC = GeqtQ(stringParameters); 

            return new ResolvableItemDescription(actionName, parametersAsNVC);
        }

        private static Dictionary<string, object> GeqtQ(in ReadOnlySpan<char> stringParameters)
        {
            var queryString = QueryHelpers.ParseQuery(stringParameters.ToString());
            return queryString.Keys.ToDictionary(_ => _, _ => (object)queryString[_]);
        }
    }
}
