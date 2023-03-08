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

namespace PowerLyrics.Windows
{
    /// <summary>
    /// Interaction logic for AudiencWindow.xaml
    /// Use singleton patter to make only one instance of this window, it is better in code (i do not need send instens over code)
    /// </summary>
    public partial class AudiencWindow : Window
    {

        //user singleton pattern to make sure only one instance of this window is created
        private static AudiencWindow _instance;

        private ContentControl content;

        public ContentControl swhoContent
        {
            get { return content; }
        }


        public static AudiencWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudiencWindow();
                }
                return _instance;
            }
        }
        private AudiencWindow()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width; // set window to right side of screen
            LyricViewTemplate1 lyricView = new LyricViewTemplate1();
            this.Control.Content = lyricView;
        }
    }
}
