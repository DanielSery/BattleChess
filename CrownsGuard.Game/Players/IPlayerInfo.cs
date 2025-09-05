// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Core.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Players;

public interface IPlayerInfo : IFigureOwner
{
    IPlayerTimer Timer { get; }
    
    string Name { get; }
    
    void SetTimer(IPlayerTimer timer);

    void StartTurn();

    void EndTurn(TimeSpan? forcedTurnDuration = null);
}