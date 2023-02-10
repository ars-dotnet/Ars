using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ars.Commom.Tool
{
    public class DisposeAction : IAsyncDisposable, IDisposable
    {
        private Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var action = Interlocked.Exchange(ref _action, null);
                action?.Invoke();
            }
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            var action = Interlocked.Exchange(ref _action, null);
            action?.Invoke();

            return ValueTask.CompletedTask;
        }
    }
}
