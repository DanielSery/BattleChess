#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

namespace BattleChess3.Core.Model;

public interface IBoard : IReadOnlyList<ITile>
{
    ITile this[Position position] { get; }
}