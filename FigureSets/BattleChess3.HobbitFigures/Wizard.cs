﻿using System;
using System.Collections.Generic;
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.HobbitFigures.Localization;

namespace BattleChess3.HobbitFigures
{
    public class Wizard : IFigureType
    {
        public static readonly Wizard Instance = new Wizard();
        public string ShownName => CurrentLocalization.Instance["Wizard_Name"];
        public string Description => CurrentLocalization.Instance["Wizard_Description"];
        public string UnitName { get; } = $"{nameof(HobbitFigureGroup)}.{nameof(Wizard)}";
        public FigureTypes UnitTypes { get; } = FigureTypes.Foot;
        public FigureTypes Bonus { get; } = FigureTypes.Nothing;
        public FigureTypes AntiBonus { get; } = FigureTypes.Nothing;
        public double Attack { get; } = 100;
        public double Defence { get; } = 0;
        public bool MovingAttack { get; } = true;
        public int Cost { get; } = 5;

        public Dictionary<int, Uri> ImageUris { get; } = new Dictionary<int, Uri>
        {
            {0, new Uri("pack://application:,,,/BattleChess3.HobbitFigures;component/Images/Wizard1.png", UriKind.Absolute)},
            {1, new Uri("pack://application:,,,/BattleChess3.HobbitFigures;component/Images/Wizard2.png", UriKind.Absolute)},
        };

        public void AttackAction(Tile from, Tile to, Tile[] board)
            => to.KillFigure(board);

        public void MoveAction(Tile from, Tile to, Tile[] board)
            => from.MoveToPosition(to.Position, board);

        private readonly Position[][] _moveChain = 
        {
            new Position[] {(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)}, 
            new Position[] {(0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7)},
            new Position[] {(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0)},
            new Position[] {(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7)}
        };
        public Position[][] GetMoveChains(Position position) => _moveChain;
        
        
        private readonly Position[][] _attackChain = 
        {
            new Position[] {(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)}, 
            new Position[] {(0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7)},
            new Position[] {(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0)},
            new Position[] {(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7)}
        };
        public Position[][] GetAttackChains(Position position) => _attackChain;
    }
}