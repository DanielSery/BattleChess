// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Core.Players;
using BattleChess3.Game.Timers;

namespace BattleChess3.Game.Players;

public interface IPlayerInfo : IFigureOwner
{
    IPlayerTimer Timer { get; }
    
    string Name { get; }
    
    void SetTimer(IPlayerTimer timer);

    void StartTurn();

    void EndTurn(TimeSpan? forcedTurnDuration = null);
}