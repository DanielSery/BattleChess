﻿using System;
using System.Linq;

namespace BattleChess3.Core.Figures.AttackingTypes
{
    /// <summary>
    /// Class of simple move of figure
    /// </summary>
    public class SimpleMoveFigure
    {
        /// <summary>
        /// Checks if position is one of possible moving positions
        /// </summary>
        public Func<BaseFigure, BaseFigure, Position[], bool> CanMoveSimple => (movingFigure, moveToFigure, avaibleMoves) =>
        {
            return avaibleMoves.Any(avaibleMove => avaibleMove == moveToFigure.Position.SubstractPositions(movingFigure.Position));
        };
    }
}