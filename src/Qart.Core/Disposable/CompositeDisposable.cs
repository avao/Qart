using System;
using System.Collections.Generic;

namespace Qart.Core.Disposable
{
    public class CompositeDisposable : IDisposable
    {
        private readonly IReadOnlyCollection<IDisposable> _disposables;

        public CompositeDisposable(IReadOnlyCollection<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
