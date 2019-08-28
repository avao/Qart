using System.Collections.Generic;
namespace Qart.Testing.Framework
{
    public class SequentialCriticalSectionTokensProvider<T> : ICriticalSectionTokensProvider<T>
    {
        private readonly IEnumerable<string> _csTokens = new[] { string.Empty };

        public System.Collections.Generic.IEnumerable<string> GetTokens(T testCase)
        {
            return _csTokens;
        }
    }
}
