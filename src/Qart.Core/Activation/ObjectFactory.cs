using Qart.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Qart.Core.Activation.ActivationAliasDescription;

namespace Qart.Core.Activation
{
    public class ObjectFactory<T> : IObjectFactory<T>
    {
        private readonly ActivationRegistry<T> _activationRegistry;
        private readonly IServiceProvider _serviceProvider;

        public ObjectFactory(IServiceProvider serviceProvider, ActivationRegistry<T> activationRegistry)
        {
            _activationRegistry = activationRegistry;
            _serviceProvider = serviceProvider;
        }

        public T Create(string name, IDictionary<string, object> parameters)
        {
            if (!_activationRegistry.TryGetValue(name, out var actionDetails))
            {
                Require.Fail($"Could not find implementation type for action [{name}]");
            }
            var enrichedParameters = actionDetails.Parameters?.Count > 0
                ? MergeLeftAllowOverrides(StringComparer.InvariantCultureIgnoreCase, actionDetails.Parameters, parameters)
                : parameters;
            return _serviceProvider.CreateInstance<T>(actionDetails.Type, enrichedParameters);
        }

        public IReadOnlyCollection<ActivationAliasDescription> GetDescriptions()
        {
            return _activationRegistry.Aliases
                .OrderBy(item => item)
                .Select(alias =>
                    {
                        _activationRegistry.TryGetValue(alias, out var activationInfo);
                        var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(activationInfo.Type, typeof(DescriptionAttribute));
                        var description = descriptionAttribute?.Description;
                        var parameterGroups = activationInfo.Type.GetConstructors()
                            .OrderBy(c => c.GetParameters().Length)
                            .Select(c => c.GetParameters()
                                .Where(p => activationInfo.Parameters?.TryGetValue(p.Name, out var _) != true)
                                .Select(p => new ParameterDescription(p.Name, p.GetCustomAttributes(typeof(DescriptionAttribute), true).OfType<DescriptionAttribute>().FirstOrDefault()?.Description, p.ParameterType.Name, p.IsOptional, p.DefaultValue))
                                .ToList()
                            ).ToList();

                        return new ActivationAliasDescription(alias, description, parameterGroups);
                    })
                .ToList();
        }



        private static IDictionary<T1, T2> MergeLeftAllowOverrides<T1, T2>(IEqualityComparer<T1> comparer, params IDictionary<T1, T2>[] dictionaries)
        {
            var result = new Dictionary<T1, T2>(comparer);
            foreach (var dictionary in dictionaries)
            {
                if (dictionary != null)
                {
                    foreach (var kvp in dictionary)
                    {
                        result[kvp.Key] = kvp.Value;
                    }
                }
            }
            return result;
        }
    }
}
