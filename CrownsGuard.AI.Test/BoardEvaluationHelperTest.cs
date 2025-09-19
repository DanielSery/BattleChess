using AwesomeAssertions;
using CrownsGuard.Core.Figures;

namespace CrownsGuard.AI.Test;

public class UnitTest1
{
    private const Figure ____ = Figure.Empty;
    private const Figure _WB_ = Figure.CamelRider | Figure.IsWhite;
    private const Figure _BB_ = Figure.CamelRider | Figure.IsBlack;
    private const Figure _WP_ = Figure.LegionarySword | Figure.IsWhite;
    private const Figure _BP_ = Figure.LegionarySword | Figure.IsBlack;
    private const Figure _WK_ = Figure.CamelRider | Figure.IsWhite;
    private const Figure _BK_ = Figure.CamelRider | Figure.IsBlack;
    
    [Fact]
    public void Empty_Zero()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().Be(0);
    }
    
    [Fact]
    public void Bishop_White()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, _WB_, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().BeLessThan(0);
    }
    
    [Fact]
    public void Bishop_Black()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, _BB_, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void Bishops_White()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, _BB_, ____, ____, ____, ____, ____, 
            ____, _WB_, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().BeLessThan(0);
    }
    
    [Fact]
    public void Bishops_Black()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, _BB_, ____, ____, ____, ____, ____, 
            ____, _WB_, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsBlack);
        
        evaluation.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void Bishops2_White()
    {
        var board = new Figure[64]
        {
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            _BB_, ____, _BB_, ____, ____, ____, ____, ____, 
            ____, _WB_, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().BeLessThan(0);
    }
    
    [Fact]
    public void Bishops4_White()
    {
        var board = new Figure[64]
        {
            ____, _BK_, ____, ____, ____, ____, ____, ____, 
            _WB_, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, _BP_, ____, 
            ____, ____, ____, ____, ____, _WK_, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsWhite);
        
        evaluation.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void Bishops4_Black()
    {
        var board = new Figure[64]
        {
            ____, _BK_, ____, ____, ____, ____, ____, ____, 
            _WB_, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, _BP_, ____, 
            ____, ____, ____, ____, ____, _WK_, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
            ____, ____, ____, ____, ____, ____, ____, ____, 
        };

        var evaluation = BoardEvaluationHelper.EvaluateBoard(board, new Stack<FigureAction>(), Figure.IsBlack);
        
        evaluation.Should().BeLessThan(0);
    }
}