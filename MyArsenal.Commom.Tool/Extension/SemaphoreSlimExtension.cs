using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ars.Commom.Tool.Extension
{
    public static class SemaphoreSlimExtension
    {
        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, Func<Task> func)
        {
            await semaphoreSlim.WaitAsync();
            await using var _ = await GetDisposeAsync(semaphoreSlim);
            await func();
        }

        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, Func<Task> func)
        {
            await semaphoreSlim.WaitAsync(cancellationToken);
            await using var _ = await GetDisposeAsync(semaphoreSlim);
            await func();
        }

        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, Func<Task> func)
        {
            if (await semaphoreSlim.WaitAsync(millisecondsTimeout))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                await func();
            }
        }

        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken, Func<Task> func)
        {
            if (await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                await func();
            }
        }

        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, Func<Task> func)
        {
            if (await semaphoreSlim.WaitAsync(timeout))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                await func();
            }
        }

        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken, Func<Task> func)
        {
            if (await semaphoreSlim.WaitAsync(timeout, cancellationToken))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                await func();
            }
        }

        public static async Task<T> LockAsync<T>(this SemaphoreSlim semaphoreSlim, Func<Task<T>> func)
        {
            await semaphoreSlim.WaitAsync();
            await using var _ = await GetDisposeAsync(semaphoreSlim);
            return await func();
        }

        public static async Task<T> LockAsync<T>(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, Func<Task<T>> func)
        {
            await semaphoreSlim.WaitAsync(cancellationToken);
            await using var _ = await GetDisposeAsync(semaphoreSlim);
            return await func();
        }

        public static async Task<T?> LockAsync<T>(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, Func<Task<T>> func)
        {
            if (await semaphoreSlim.WaitAsync(millisecondsTimeout))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                return await func();
            }

            return default(T);
        }

        public static async Task<T?> LockAsync<T>(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken, Func<Task<T>> func)
        {
            if (await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                return await func();
            }

            return default(T);
        }

        public static async Task<T?> LockAsync<T>(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, Func<Task<T>> func)
        {
            if (await semaphoreSlim.WaitAsync(timeout))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                return await func();
            }

            return default(T);
        }

        public static async Task<T?> LockAsync<T>(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken, Func<Task<T>> func)
        {
            if (await semaphoreSlim.WaitAsync(timeout, cancellationToken))
            {
                await using var _ = await GetDisposeAsync(semaphoreSlim);
                return await func();
            }

            return default(T);
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, Action action)
        {
            semaphoreSlim.Wait();
            using var _ = GetDispose(semaphoreSlim);
            action();
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, Action action)
        {
            semaphoreSlim.Wait(cancellationToken);
            using var _ = GetDispose(semaphoreSlim);
            action();
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, Action action)
        {
            if (semaphoreSlim.Wait(millisecondsTimeout))
            {
                using var _ = GetDispose(semaphoreSlim);
                action();
            }
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken, Action action)
        {
            if (semaphoreSlim.Wait(millisecondsTimeout, cancellationToken))
            {
                using var _ = GetDispose(semaphoreSlim);
                action();
            }
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, Action action)
        {
            if (semaphoreSlim.Wait(timeout))
            {
                using var _ = GetDispose(semaphoreSlim);
                action();
            }
        }

        public static void Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken, Action action)
        {
            if (semaphoreSlim.Wait(timeout, cancellationToken))
            {
                using var _ = GetDispose(semaphoreSlim);
                action();
            }
        }

        private static DisposeAction GetDispose(this SemaphoreSlim semaphoreSlim)
        {
            return new DisposeAction(() => semaphoreSlim.Release());
        }

        private static Task<DisposeAction> GetDisposeAsync(this SemaphoreSlim semaphoreSlim)
        {
            return Task.FromResult(new DisposeAction(() => semaphoreSlim.Release()));
        }
    }
}
