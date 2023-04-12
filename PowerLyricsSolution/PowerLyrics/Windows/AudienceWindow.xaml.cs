using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfScreenHelper;
using WpfScreenHelper.Enum;

namespace PowerLyrics.Windows;

/// <summary>
///     Interaction logic for AudiencWindow.xaml
/// </summary>
public partial class AudiencWindow : Window
{
    public AudiencWindow()
    {
        InitializeComponent();
        //this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width; // set window to right side of screen
        WindowState = WindowState.Minimized;
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    public void setFullScrean()
    {
        //this.WindowState = WindowState.Maximized;
        var screens = Screen.AllScreens.ToArray();
        if (screens.Length > 1)
        {
            WindowState = WindowState.Normal;
            this.SetWindowPosition(WindowPositions.Maximize, screens.ElementAt(1));
        }
    }
}