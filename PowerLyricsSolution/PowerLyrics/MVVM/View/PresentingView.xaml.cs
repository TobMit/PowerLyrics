using System.Windows.Controls;
using System.Windows.Input;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.MVVM.View;

public partial class PresentingView : UserControl
{
    public PresentingView()
    {
        InitializeComponent();
    }

    public PresentingViewModel getDataContext()
    {
        return presentingDataContext;
    }

    private void ListViewSlides_OnKeyUp(object sender, KeyEventArgs e)
    {
        presentingDataContext.key(e);
    }
}