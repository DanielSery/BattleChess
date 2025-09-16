// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;

namespace CrownsGuard.Core.Helpers;

public static class PositionsHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte GetRelativeX(short relative)
    {
        return (sbyte) (relative & 255);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte GetRelativeY(short relative)
    {
        return (sbyte) (relative >> 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetAbsoluteX(int absolute)
    {
        return (byte) (absolute & 7);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetAbsoluteY(int absolute)
    {
        return (short) (absolute >> 3);
    }
    
    public static short GetRelative(byte start, byte end)
    {
        var diff = end - start;
        return (short)(diff & 7 + (diff & ~7) << 5);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetRelativePosition(int x, int y)
    {
        return (short)(x + y << 8);
    }
    
    public static int GetWithOffset(this byte absoluteIndex, short relative)
    {
        var result = absoluteIndex + (sbyte)(relative & 255);
        if ((absoluteIndex >> Constants.BoardLengthShift) != (result >> Constants.BoardLengthShift))
        {
            return -1;
        }

        result += (relative >> 5) & ~7;
        if ((uint)result >= Constants.FullBoardTilesCount)
        {
            return -1;
        }

        return result;
    }
    
    public static int GetWithOffset(this int absoluteIndex, short relative)
    {
        var result = absoluteIndex + (sbyte)(relative & 255);
        if ((absoluteIndex >> Constants.BoardLengthShift) != (result >> Constants.BoardLengthShift))
        {
            return -1;
        }

        result += (relative >> 5) & ~7;
        if ((uint)result >= Constants.FullBoardTilesCount)
        {
            return -1;
        }

        return result;
    }
}