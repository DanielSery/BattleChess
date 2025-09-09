// Copyright (c) Veeam Software Group GmbH

using System.Buffers;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class ArrayPoolHelper
{
    public static FigureAction[] ToArrayPool(this ref Span<FigureAction> actions, int count)
    {
        var array = ArrayPool<FigureAction>.Shared.Rent(count);
        actions.CopyTo(array);
        return array;
    }
}