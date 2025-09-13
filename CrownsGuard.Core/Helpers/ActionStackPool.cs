using System.Collections.Concurrent;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Helpers;

public static class ActionStackPool
{
    private static readonly ConcurrentBag<Stack<FigureAction>> Pool = [];

    public static Rented Rent(out Stack<FigureAction> result)
    {
        if (Pool.TryTake(out var actionStack))
        {
            result = actionStack;
            result.Clear();
            return new Rented(actionStack);
        }

        result = new Stack<FigureAction>(64);
        return new Rented(result);
    }

    private static void Return(Stack<FigureAction> actionStack)
    {
        Pool.Add(actionStack);
    }

    public readonly ref struct Rented : IDisposable
    {
        private readonly Stack<FigureAction> _pool;

        public Rented(Stack<FigureAction> pool)
        {
            _pool = pool;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Return(_pool);
        }
    }
}