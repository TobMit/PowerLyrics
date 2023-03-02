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

namespace PowerLyrics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public String Sirka { get; set; }
        public String Vyska { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
        }
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Vyska = "Vyska: " + e.NewSize.Height.ToString();
            Sirka = "Sirka: " + e.NewSize.Width.ToString();
        }

    }
}
