#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using System.Linq;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

public class FigureGroupViewModel : IFigureGroup
{
    public FigureGroupViewModel(IFigureGroup figureGroup)
    {
        DisplayName = figureGroup.DisplayName;
        FigureTypes = figureGroup.FigureTypes.Select(x => new FigureViewModel(x)).ToArray();
    }
    
    public string DisplayName { get; }
    public IFigureType[] FigureTypes { get; }
}