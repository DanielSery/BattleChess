#region Copyright FEI Company 2024

// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.

#endregion

using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DoubleChessFigures;

public class KnightPawn : IDoubleChessFigureType
{
    public static readonly KnightPawn Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return (this as IDoubleChessFigureType).CreatedCombinedActions(
            unitTile, 
            targetTile, 
            board,
            SingleKnight.Instance, 
            SinglePawn.Instance);
    }
}