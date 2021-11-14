using System.Collections.Generic;

namespace Qart.Core.Activation
{
    public interface IObjectFactory<T>
    {
        T Create(string alias, IDictionary<string, object> arguments);
        IReadOnlyCollection<ActivationAliasDescription> GetDescriptions();
    }

    public static class ObjectFactoryExtensions
    {
        public static T Create<T>(this IObjectFactory<T> objectFactory, string alias)
             => objectFactory.Create(alias, null);
    }
}
