// Copyright (c) Veeam Software Group GmbH

using System.Buffers;

namespace CrownsGuard.Core.Helpers;

public static class ArrayPoolHelper
{
    public static ArrayPoolMemory<T> Rent<T>(int count)
    {
        var array = ArrayPool<T>.Shared.Rent(count);
        return new ArrayPoolMemory<T>(array, count);
    }
    
    public static ArrayPoolMemory<T> ToArrayPoolMemory<T>(this Span<T> actions, int count)
    {
        var array = ArrayPool<T>.Shared.Rent(count);
        actions[..count].CopyTo(array);
        return new ArrayPoolMemory<T>(array, count);
    }
}