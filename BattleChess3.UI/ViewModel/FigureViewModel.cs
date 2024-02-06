#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

public class FigureViewModel : IFigureType
{
    private readonly Func<ITile, ITile, ITile[], FigureAction> _getActionsFunc;

    public FigureViewModel(IFigureType figureType)
    {
        _getActionsFunc = figureType.GetPossibleAction;
        
        DisplayName = figureType.DisplayName;
        Description = figureType.Description;
        UnitName = figureType.UnitName;
        ImageUris = figureType.ImageUris;
    }

    public string DisplayName { get; }
    public string Description { get; }
    public string UnitName { get; }
    public IDictionary<int, Uri> ImageUris { get; }
    
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return _getActionsFunc.Invoke(unitTile, targetTile, board);
    }
}