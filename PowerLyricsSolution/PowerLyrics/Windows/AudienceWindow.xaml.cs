using PowerLyrics.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Windows
{
    /// <summary>
    /// Interaction logic for AudiencWindow.xaml
    /// </summary>
    public partial class AudiencWindow : Window
    {
        
        public AudiencWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width; // set window to right side of screen
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void setFullScrean()
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}
