using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.MVVM.View;

public partial class EditView : UserControl
{
    private Regex number;
    public EditView()
    {
        InitializeComponent();
        number = new Regex("[^0-9]+");
    }
    public EditViewModel getDataContext()
    {
        return this.editDataContext;
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = number.IsMatch(e.Text);
    }
}