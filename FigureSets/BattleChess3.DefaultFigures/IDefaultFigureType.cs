#region Copyright FEI Company 2024

// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.

#endregion

using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Localization;

namespace BattleChess3.DefaultFigures;

public interface IDefaultFigureType : IFigureType
{
    string IFigureType.UnitName => $"{nameof(DefaultFigureGroup)}.{GetType().Name}";
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(DisplayName)}"];
    string IFigureType.Description => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(Description)}"];

    IDictionary<int, Uri> IFigureType.ImageUris => new Dictionary<int, Uri>
    {
        {
            0,
            new Uri($"pack://application:,,,/BattleChess3.DefaultFigures;component/Images/{GetType().Name}0.png",
                UriKind.Absolute)
        }
    };
}