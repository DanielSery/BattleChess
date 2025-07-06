using System.IO;
using System.Text;
using System.Windows;
using BattleChess3.UI.ViewModel;

namespace BattleChess3.UI.Views;

/// <summary>
///     Interaction logic for MultiplayerControl.xaml
/// </summary>
public partial class MultiplayerControl
{
    public MultiplayerControl()
    {
        InitializeComponent();
    }

    private void Button_Drop(object sender, DragEventArgs e)
    {
        if (!(e.Data.GetData("text") is MemoryStream ms))
        {
            return;
        }

        var text = Encoding.ASCII.GetString(ms.ToArray());
        var viewModel = DataContext as MultiplayerViewModel;

        if (viewModel is null)
        {
            return;
        }

        if (!uint.TryParse(text, out var gameId))
            return;
        
        // viewModel.SetGameId(gameId);
    }

    private void Button_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
    }
}