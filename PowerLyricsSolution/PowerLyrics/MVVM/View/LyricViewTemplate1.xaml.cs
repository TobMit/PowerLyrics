using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.View
{
    /// <summary>
    /// Interaction logic for LyricViewTemplate1.xaml
    /// </summary>
    public partial class LyricViewTemplate1 : UserControl
    {
        public string text
        {
            get
            {
                return TextBlock.Text;
            }
            set
            {
                TextBlock.Text = value;
            }
        }

        public double fontSize
        {
            get
            {
                return TextBlock.FontSize;
            }
            set
            {
                TextBlock.FontSize = value;
            }
        }

        public LyricViewTemplate1()
        {
            InitializeComponent();
        }

        public LyricViewTemplate1(LyricViewTemplate1 copy)
        {
            InitializeComponent();
            this.text = copy.text;
            this.fontSize = copy.fontSize;
        }

        public LyricViewTemplate1(string text)
        {
            InitializeComponent();
            this.text = text;
        }
    }
}
