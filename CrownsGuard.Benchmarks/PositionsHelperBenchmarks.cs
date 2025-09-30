using System;
using BenchmarkDotNet.Attributes;
using CrownsGuard.Core;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 2)]
public class PositionsHelperBenchmarks
{
    [Params(1024, 16384, 262144)]
    public int Iterations { get; set; }

    private byte[] _abs8 = default!;
    private int[] _abs32 = default!;
    private short[] _rel = default!;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _abs8 = new byte[Iterations];
        _abs32 = new int[Iterations];
        _rel = new short[Iterations];

        for (int i = 0; i < Iterations; i++)
        {
            int a = rnd.Next(Constants.FullBoardTilesCount);
            _abs8[i] = (byte)a;
            _abs32[i] = a;

            // random dx in [-3,3], dy in [-3,3] so we have both valid and invalid
            int dx = rnd.Next(-3, 4);
            int dy = rnd.Next(-3, 4);
            _rel[i] = PositionsHelper.GetRelativePosition(dx, dy);
        }
    }

    [Benchmark(Baseline = true)]
    public int New_Int()
    {
        int sum = 0;
        var rel = _rel;
        var abs = _abs32;
        for (int i = 0; i < abs.Length; i++)
        {
            sum += PositionsHelper.GetWithOffset(abs[i], rel[i]);
        }
        return sum;
    }

    [Benchmark]
    public int New_Byte()
    {
        int sum = 0;
        var rel = _rel;
        var abs = _abs8;
        for (int i = 0; i < abs.Length; i++)
        {
            sum += PositionsHelper.GetWithOffset(abs[i], rel[i]);
        }
        return sum;
    }

    [Benchmark]
    public int Old_Int()
    {
        int sum = 0;
        var rel = _rel;
        var abs = _abs32;
        for (int i = 0; i < abs.Length; i++)
        {
            sum += OldGetWithOffset(abs[i], rel[i]);
        }
        return sum;
    }

    [Benchmark]
    public int Old_Byte()
    {
        int sum = 0;
        var rel = _rel;
        var abs = _abs8;
        for (int i = 0; i < abs.Length; i++)
        {
            sum += OldGetWithOffset(abs[i], rel[i]);
        }
        return sum;
    }

    // Old logic copied here for comparison
    private static int OldGetWithOffset(int absoluteIndex, short relative)
    {
        int result = absoluteIndex + (sbyte)(relative & 255);
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