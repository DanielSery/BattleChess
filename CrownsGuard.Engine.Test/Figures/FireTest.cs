namespace CrownsGuard.Engine.Test.Figures;

/// <summary>
/// Not working code in engine
/// </summary>
public class FireTest
{
    // [Fact]
    // public Task OnDied_NonDragon_FireDisappears()
    // {
    //     const int position = 27; // d4
    //     var board = new Figure[64];
    //     board[position] = Figure.Fire;
    //
    //     // Test various non-dragon figures
    //     var figuresToTest = new[]
    //     {
    //         Figure.Peasant | Figure.IsWhite,
    //         Figure.Archer | Figure.IsBlack,
    //         Figure.Knight | Figure.IsWhite,
    //         Figure.Mage | Figure.IsBlack,
    //         Figure.Builder | Figure.IsWhite,
    //         Figure.Wall,
    //         Figure.Explosives | Figure.IsBlack
    //     };
    //
    //     foreach (var figure in figuresToTest)
    //     {
    //         var testBoard = board.ToArray();
    //         testBoard[position] = figure; // Place the figure on fire
    //
    //         var events = new List<BoardEvent>();
    //         Fire.OnDied(new BoardEvent(BoardEventType.Died, position, position), testBoard.AsSpan(), (e, _) => events.Add(e));
    //
    //         // Fire should disappear (become empty)
    //         Assert.Equal(Figure.Empty, testBoard[position]);
    //         Assert.Single(events);
    //         Assert.Equal(BoardEventType.Died, events[0].EventType);
    //         Assert.Equal(position, events[0].SourceIndex);
    //     }
    //
    //     return Task.CompletedTask;
    // }
    //
    // [Fact]
    // public void OnDied_Dragon_FireRemains()
    // {
    //     const int position = 27; // d4
    //     var board = new Figure[64];
    //     board[position] = Figure.Fire;
    //
    //     var dragon = Figure.Dragon | Figure.IsWhite;
    //     board[position] = dragon; // Place dragon on fire
    //
    //     var events = new List<BoardEvent>();
    //     Fire.OnDied(new BoardEvent(BoardEventType.Died, position, position), board.AsSpan(), (e, _) => events.Add(e));
    //
    //     // Fire should remain when dragon dies
    //     Assert.Equal(Figure.Fire, board[position]);
    //     Assert.Empty(events); // No events should be raised
    // }
    //
    // [Fact]
    // public void OnDied_DragonBlack_FireRemains()
    // {
    //     const int position = 27; // d4
    //     var board = new Figure[64];
    //     board[position] = Figure.Fire;
    //
    //     var dragon = Figure.Dragon | Figure.IsBlack;
    //     board[position] = dragon; // Place black dragon on fire
    //
    //     var events = new List<BoardEvent>();
    //     Fire.OnDied(new BoardEvent(BoardEventType.Died, position, position), board.AsSpan(), (e, _) => events.Add(e));
    //
    //     // Fire should remain when black dragon dies
    //     Assert.Equal(Figure.Fire, board[position]);
    //     Assert.Empty(events); // No events should be raised
    // }
    //
    // [Fact]
    // public void OnDied_EmptyTile_NoAction()
    // {
    //     const int position = 27; // d4
    //     var board = new Figure[64];
    //     board[position] = Figure.Empty; // No fire
    //
    //     var events = new List<BoardEvent>();
    //     Fire.OnDied(new BoardEvent(BoardEventType.Died, position, position), board.AsSpan(), (e, _) => events.Add(e));
    //
    //     // Should not affect empty tiles
    //     Assert.Equal(Figure.Empty, board[position]);
    //     Assert.Empty(events);
    // }
}