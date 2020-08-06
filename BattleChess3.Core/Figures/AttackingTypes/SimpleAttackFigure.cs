﻿// using System;
// using System.Linq;
//
// namespace BattleChess3.Core.Figures.AttackingTypes
// {
//     /// <summary>
//     /// Class of simple attack of figure
//     /// </summary>
//     public class SimpleAttackFigure : SimpleMoveFigure
//     {
//         /// <summary>
//         /// Checks if position is one of possible attacking positions
//         /// </summary>
//         public Func<Figure, Figure, Position[], bool> CanAttackSimple => (figure, attackFigure, avaibleAttacks) =>
//         {
//             return avaibleAttacks.Any(avaibleAttack => avaibleAttack == attackFigure.Position.SubtractPositions(figure.Position));
//         };
//     }
// }