using System.Numerics;
using System.Runtime.InteropServices;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.AI;

public static class FigureArraySimdHelper
{
    public static int SimdSum(ReadOnlySpan<Figure> figures)
    {
        // Reinterpret Figure as ushort
        var span = MemoryMarshal.Cast<Figure, ushort>(figures);

        var vectorSize = Vector<ushort>.Count;
        var i = 0;
        var vsum = Vector<ushort>.Zero;

        // SIMD loop
        for (; i <= span.Length - vectorSize; i += vectorSize)
        {
            vsum += new Vector<ushort>(span.Slice(i, vectorSize));
        }

        // Horizontal add of vector lanes
        var sum = 0;
        for (var j = 0; j < vectorSize; ++j)
            sum += vsum[j];

        // Remainder
        for (; i < span.Length; ++i)
            sum += span[i];

        return sum;
    }
    
    public static long FastSimdHash64(ReadOnlySpan<Figure> figures)
    {
        // Safely reinterpret Figure as ushort
        var span = MemoryMarshal.Cast<Figure, ushort>(figures);

        const ulong fnvOffset = 14695981039346656037UL;
        const ulong fnvPrime  = 1099511628211UL;

        var hash1 = fnvOffset;
        var hash2 = fnvOffset ^ 0xA5A5A5A5A5A5A5A5UL;

        var vectorSize = Vector<ushort>.Count;
        var i = 0;

        // SIMD part
        for (; i <= span.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<ushort>(span.Slice(i, vectorSize));
            for (var j = 0; j < vectorSize; ++j)
            {
                hash1 ^= v[j];
                hash1 *= fnvPrime;
                hash2 += v[j];
                hash2 *= (fnvPrime ^ 0x5B5B5B5B5B5B5B5BUL);
            }
        }

        // Scalar fallback
        for (; i < span.Length; ++i)
        {
            hash1 ^= span[i];
            hash1 *= fnvPrime;
            hash2 += span[i];
            hash2 *= (fnvPrime ^ 0x5B5B5B5B5B5B5B5BUL);
        }

        return (long)(hash1 ^ (hash2 << 1));
    }
}