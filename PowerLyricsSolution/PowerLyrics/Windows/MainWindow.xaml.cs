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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        
        AudiencWindow window = AudiencWindow.Instance;

        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
        }
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Vyska.Content = "Vyska: " + e.NewSize.Height.ToString();
            Sirka.Content = "Sirka: " + e.NewSize.Width.ToString();
            window.Show();

            LyricViewTemplate1 lyricView = new LyricViewTemplate1();
            Control.Content = lyricView;
            //window.swhoContent.Content = lyricView;

        }

        protected override void OnClosed(EventArgs e)
        {
            window.Close();
            window = null;
            base.OnClosed(e);
        }
    }
}
