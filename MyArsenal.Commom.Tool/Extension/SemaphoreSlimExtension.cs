using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ars.Commom.Tool.Extension
{
    public static class SemaphoreSlimExtension
    {
        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim)
        {
            await semaphoreSlim.WaitAsync();
            return GetDispose(semaphoreSlim);
        }

        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync(cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
        {
            await semaphoreSlim.WaitAsync(millisecondsTimeout);
            return GetDispose(semaphoreSlim);
        }

        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
        {
            await semaphoreSlim.WaitAsync(timeout);
            return GetDispose(semaphoreSlim);
        }

        public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync(timeout, cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim)
        {
            semaphoreSlim.Wait();
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
        {
            semaphoreSlim.Wait(cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
        {
            semaphoreSlim.Wait(millisecondsTimeout);
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            semaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
        {
            semaphoreSlim.Wait(timeout);
            return GetDispose(semaphoreSlim);
        }

        public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
        {
            semaphoreSlim.Wait(timeout, cancellationToken);
            return GetDispose(semaphoreSlim);
        }

        private static DisposeAction GetDispose(this SemaphoreSlim semaphoreSlim)
        {
            return new DisposeAction(() => Task.FromResult(semaphoreSlim.Release()));
        }
    }
}
