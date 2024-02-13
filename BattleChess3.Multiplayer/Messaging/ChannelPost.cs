#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using System.Diagnostics.CodeAnalysis;

namespace BattleChess3.Multiplayer.Messaging;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal abstract class ChannelPost
{
    public long message_id { get; set; }
    public object? sender_chat { get; set; }
    public object? chat { get; set; }
    public long date { get; set; }
    public string? text { get; set; }
}