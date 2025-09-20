// Copyright (c) Veeam Software Group GmbH

using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.Engine.Helpers;

public static class FigureActionHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsExecutable(this FigureActionType figureActionType)
    {
        return figureActionType.HasFlag(FigureActionType.IsExecutable);
    }
}