// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Maps.Figures;

public interface IFigureWithInfo : IFigureInfo
{
    public IPlayer Owner { get; }
    public IFigureTypeInfo TypeInfo { get; }
    public bool IsKing { get; }
}