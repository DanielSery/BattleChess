// Copyright (c) Veeam Software Group GmbH

using System.Buffers;

namespace CrownsGuard.Core.Helpers;

public readonly struct ArrayPoolMemory<T> : IDisposable
{
    public static readonly ArrayPoolMemory<T> Empty = new ArrayPoolMemory<T>([], 0);
    
    private readonly Memory<T> _memory;
    private readonly T[] _array;

    public ArrayPoolMemory(T[] array, int length)
    {
        _array = array;
        _memory = new Memory<T>(array, 0, length);
    }
    
    public Span<T> Span => _memory.Span;
    public int Length => _memory.Length;

    public void Dispose()
    {
        if (_array == Array.Empty<T>())
            return;
        
        ArrayPool<T>.Shared.Return(_array);
    }
}