﻿#region Copyright FEI Company 2024

// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.

#endregion

using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.DoubleChessFigures.Localization;

namespace BattleChess3.DoubleChessFigures;

public class CombinedChessFigureType<T1, T2> : IFigureType
    where T1 : IFigureType, new()
    where T2 : IFigureType, new()
{
    private readonly T1 _firstFigure;
    private readonly T2 _secondFigure;
    private readonly string _unitName;

    public CombinedChessFigureType(string unitName)
    {
        _unitName = unitName;
        _firstFigure = new T1();
        _secondFigure = new T2();
    }

    public string UnitName => $"{nameof(DoubleChessFigureGroup)}.{_unitName}";
    public string DisplayName => CurrentLocalization.Instance[$"{_unitName}_Name"];
    public string Description => CurrentLocalization.Instance[$"{_unitName}_{nameof(Description)}"];

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{_unitName}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{_unitName}2.png", UriKind.Absolute) }
        };

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var firstUnitAction = _firstFigure.GetPossibleAction(unitTile, targetTile, board);
        if (firstUnitAction.ActionType != FigureActionTypes.None)
        {
            return new FigureAction(firstUnitAction.ActionType, () =>
            {
                var owner = unitTile.Figure.Owner;
                unitTile.Die();
                unitTile.CreateFigure(new Figure(owner, _firstFigure));
                firstUnitAction.Action.Invoke();
                unitTile.CreateFigure(new Figure(owner, _secondFigure));
            });
        }

        var secondUnitAction = _secondFigure.GetPossibleAction(unitTile, targetTile, board);
        if (secondUnitAction.ActionType != FigureActionTypes.None)
        {
            return new FigureAction(secondUnitAction.ActionType, () =>
            {
                var owner = unitTile.Figure.Owner;
                unitTile.Die();
                unitTile.CreateFigure(new Figure(owner, _secondFigure));
                secondUnitAction.Action.Invoke();
                unitTile.CreateFigure(new Figure(owner, _firstFigure));
            });
        }

        return new FigureAction(FigureActionTypes.None, () => { });
    }
}