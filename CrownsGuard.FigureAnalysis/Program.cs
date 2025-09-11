// See https://aka.ms/new-console-template for more information

using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

var board = new Figure[64];
var result = new int[64];

for (var i = 0; i < result.Length; i++)
{
    for (var j = 0; j < board.Length; j++)
    {
        board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
    }

    board[i] = new Figure(PlayerColor.White, false, FigureId.Archer);
    using var possibleActions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
    var impact = 0;
    foreach (var possibleAction in possibleActions.Span)
    {
        impact += ActionImpactEvaluator.EvaluateAction(board, possibleAction, board[i]);
    }
    result[i] = impact;
}

Console.WriteLine(result);