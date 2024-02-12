#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using BattleChess3.Core.Model.Figures;

namespace BattleChess3.Core.Model;

public class NoneTile : ITile
{
    public static readonly ITile Instance = new NoneTile();
    
    public Position Position { get; } = new Position();
    public Position AbsolutePosition { get; } = new Position();
    public Figure Figure { get; set; } = Figure.None;
    
    public ITile GetPovTile(Player player)
    {
        return Instance;
    }
}