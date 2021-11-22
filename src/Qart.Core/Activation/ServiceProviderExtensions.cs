using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qart.Core.Activation
{
    public static class ServiceProviderExtensions
    {
        public static T CreateInstance<T>(this IServiceProvider serviceProvider, IDictionary<string, object> parameterValues)
            => serviceProvider.CreateInstance<T>(typeof(T), parameterValues);

        public static T CreateInstance<T>(this IServiceProvider serviceProvider, ActivationInfo typeActivationInfo)
            => serviceProvider.CreateInstance<T>(typeActivationInfo.Type, typeActivationInfo.Parameters);

        public static T CreateInstance<T>(this IServiceProvider serviceProvider, Type type, IDictionary<string, object> parameterValues)
        {
            var caseInsensitiveParameterValues = parameterValues == null
                ? new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)//TODO avoid allocation
                : new Dictionary<string, object>(parameterValues, StringComparer.InvariantCultureIgnoreCase);
            foreach (var ctor in type.GetConstructors().OrderBy(c => c.GetParameters().Length))
            {
                if (TryResolvingParameters(ctor, caseInsensitiveParameterValues, serviceProvider, out var resolvedParameters))
                {
                    return (T)ctor.Invoke(resolvedParameters);
                }
            }
            throw new NotSupportedException($"Could not create {type} with parameter overrides {caseInsensitiveParameterValues.Keys.ToCsv()}");
        }

        private static bool TryResolvingParameters(ConstructorInfo constructorInfo, IDictionary<string, object> parameterValues, IServiceProvider serviceProvider, out object[] resolvedParameters)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length < parameterValues.Count)
            {
                resolvedParameters = default;
                return false;
            }

            resolvedParameters = new object[parameters.Length];
            var matchedParameterValues = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameterValues.TryGetValue(parameter.Name, out var value))
                {
                    if (value is not null && parameter.ParameterType.IsAssignableFrom(value.GetType())
                        || !parameter.ParameterType.IsValueType
                        || System.Nullable.GetUnderlyingType(parameter.ParameterType) != null
                        || TryChangeType(value, parameter.ParameterType, out value))
                    {
                        ++matchedParameterValues;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (parameter.IsOptional)
                {
                    value = parameter.DefaultValue;
                }
                else
                {
                    value = serviceProvider.GetService(parameter.ParameterType);
                    if (value == null)
                        return false;
                }

                resolvedParameters[i] = value;
            }
            return matchedParameterValues == parameterValues.Count;
        }

        private static bool TryChangeType(object value, Type conversionType, out object convertedValue)
        {
            if (conversionType != null
                && value != null
                && value is IConvertible)
            {
                try
                {
                    convertedValue = Convert.ChangeType(value, conversionType);
                    return true;
                }
                catch { }
            }
            convertedValue = default;
            return false;
        }
    }
}
