using CrownsGuard.Core;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CrownsGuard.Engine.Helpers;

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
        var relativeStart = PositionHelpStruct.FromAbsolute(start);
        var relativeEnd = PositionHelpStruct.FromAbsolute(end);
        var relative = new PositionHelpStruct((sbyte)(relativeEnd.X - relativeStart.X), (sbyte)(relativeEnd.Y - relativeStart.Y));
        return relative.Position;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetRelativePosition(int x, int y)
    {
        var relative = new PositionHelpStruct((sbyte)x, (sbyte)y);
        return relative.Position;
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
    
    [StructLayout(LayoutKind.Explicit)]
    internal struct PositionHelpStruct
    {
        [FieldOffset(0)]
        public sbyte X;

        [FieldOffset(1)]
        public sbyte Y;

        [FieldOffset(0)]
        public short Position;

        public PositionHelpStruct(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public PositionHelpStruct(short position)
        {
            Position = position;
        }

        public static PositionHelpStruct FromAbsolute(short absolute)
        {
            return new PositionHelpStruct(
                (sbyte)(absolute & 7),
                (sbyte)(absolute >> 3));
        }
    }
}