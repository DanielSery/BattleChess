﻿using System;
using System.Collections.Generic;
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.LordOfTheRingsFigures.Localization;

namespace BattleChess3.LordOfTheRingsFigures;

public class SoldierOrc : IFigureType
{
    public static readonly SoldierOrc Instance = new();
    public string ShownName => CurrentLocalization.Instance["SoldierOrc_Name"];
    public string Description => CurrentLocalization.Instance["SoldierOrc_Description"];
    public string UnitName => $"{nameof(LordOfTheRingsFigureGroup)}.{nameof(SoldierOrc)}";
    public FigureTypes UnitType => FigureTypes.Foot;
    public double FullHp => 100;
    public double Attack => 100;
    public int Cost => 1;

    public Dictionary<int, Uri> ImageUris { get; } = new Dictionary<int, Uri>
    {
        {1, new Uri("pack://application:,,,/BattleChess3.LordOfTheRingsFigures;component/Images/SoldierOrc1.png", UriKind.Absolute)},
        {2, new Uri("pack://application:,,,/BattleChess3.LordOfTheRingsFigures;component/Images/SoldierOrc2.png", UriKind.Absolute)},
    };

    public double AttackCalculation(IFigureType figureType)
        => figureType.DefenceCalculation(this);

    public double DefenceCalculation(IFigureType figureType)
        => figureType.Attack;

    public void AttackAction(ITile from, ITile to, ITile[] board)
        => board.KillFigureWithMove(from, to);

    public void MoveAction(ITile from, ITile to, ITile[] board)
        => board.MoveToPosition(from, to.Position);

    private readonly Position[][] _firstMoveChain = 
    {
        new Position[] {(1, 0), (2, 0)},
    };
    private readonly Position[][] _moveChain = 
    {
        new Position[] {(1, 0)},
    };
    public Position[][] GetMoveChains(Position position, ITile[] board) 
        => position.Y == 1 ? _firstMoveChain : _moveChain;


    private readonly Position[][] _attackChain = 
    {
        new Position[] {(1, 1)},
        new Position[] {(1, -1)},
    };
    public Position[][] GetAttackChains(Position position, ITile[] board) => _attackChain;
}
