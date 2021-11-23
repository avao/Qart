using Qart.Core.Text;
using Qart.Core.Validation;
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
                    if (!TryChangeType(value, parameter.ParameterType, out value))
                        return false;

                    ++matchedParameterValues;
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

        private static bool TryChangeType(object value, Type targetType, out object convertedValue)
        {
            Require.NotNull(targetType, "Target type cannot be null");
                       

            if (value == null)
            {
                if (!targetType.IsValueType
                    || System.Nullable.GetUnderlyingType(targetType) != null)
                {
                    convertedValue = null;
                    return true;
                }
            }
            else
            {
                var underlyingType = System.Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    return TryChangeType(value, underlyingType, out convertedValue);
                }

                if (targetType.IsAssignableFrom(value.GetType()))
                {
                    convertedValue = value;
                    return true;
                }
                else if (value is IConvertible convertable)
                {
                    try
                    {
                        convertedValue = convertable.ToType(targetType, null);
                        return true;
                    }
                    catch { }
                }
            }
            
            convertedValue = default;
            return false;
        }
    }
}
