using System.Windows.Controls;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.MVVM.View;

public partial class EditView : UserControl
{
    public EditView()
    {
        InitializeComponent();
    }
    public EditViewModel getDataContext()
    {
        return this.editDataContext;
    }
}