#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

namespace BattleChess3.Core.Model.Figures;

public class FigureAction
{
    public static readonly FigureAction None = new FigureAction(FigureActionTypes.None, () => { });
    
    public FigureAction(FigureActionTypes actionType, Action action)
    {
        ActionType = actionType;
        Action = action;
    }
    
    public FigureActionTypes ActionType { get; }
    public Action Action { get; }
}