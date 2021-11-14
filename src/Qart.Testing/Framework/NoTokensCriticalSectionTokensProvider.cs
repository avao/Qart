using System;
using System.Collections.Generic;
namespace Qart.Testing.Framework
{
    public class NoTokensCriticalSectionTokensProvider<T> : ICriticalSectionTokensProvider<T>
    {
        public IReadOnlyCollection<string> GetTokens(T item)
        {
            return Array.Empty<string>();
        }
    }
}
