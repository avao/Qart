using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public interface ICriticalSectionTokensProvider<T>
    {
        IReadOnlyCollection<string> GetTokens(T item);
    }
}
