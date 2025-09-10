// Copyright (c) Veeam Software Group GmbH

using System.Buffers;
using System.Runtime.CompilerServices;

namespace CrownsGuard.Core.Helpers;

public static class ArrayPoolHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayPoolMemory<T> Rent<T>(int count)
    {
        var array = ArrayPool<T>.Shared.Rent(count);
        return new ArrayPoolMemory<T>(array, count);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayPoolMemory<T> ToArrayPoolMemory<T>(this Span<T> actions, int count)
    {
        var array = ArrayPool<T>.Shared.Rent(count);
        actions[..count].CopyTo(array);
        return new ArrayPoolMemory<T>(array, count);
    }
}