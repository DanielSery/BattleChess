using CrownsGuard.Core;
using System.Collections.Concurrent;

namespace CrownsGuard.AI.Helpers;

public static class BoardPool<T>
{
    private static readonly ConcurrentBag<T[]> Pool = [];

    public static Rented<T> Rent(out T[] result)
    {
        if (Pool.TryTake(out var actionStack))
        {
            result = actionStack;
            return new Rented<T>(actionStack);
        }

        result = new T[Constants.FullBoardTilesCount];
        return new Rented<T>(result);
    }

    public static void Return(T[] actionStack)
    {
        Pool.Add(actionStack);
    }

    public readonly ref struct Rented<T1> : IDisposable
    {
        private readonly T1[] _pool;

        public Rented(T1[] pool)
        {
            _pool = pool;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            BoardPool<T1>.Return(_pool);
        }
    }
}