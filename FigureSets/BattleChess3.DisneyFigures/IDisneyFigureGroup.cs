#region Copyright FEI Company 2024

// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.

#endregion

using BattleChess3.Core.Model.Figures;
using BattleChess3.DisneyFigures.Localization;

namespace BattleChess3.DisneyFigures;

internal interface IDisneyFigureGroup : IFigureType
{
    string IFigureType.UnitName => $"{nameof(DisneyFigureGroup)}.{GetType().Name}";
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_Name"];
    string IFigureType.Description => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(Description)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/BattleChess3.DisneyFigures;component/Images/{GetType().Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/BattleChess3.DisneyFigures;component/Images/{GetType().Name}2.png", UriKind.Absolute) }
        };
}