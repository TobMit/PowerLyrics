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

namespace TwoFromWPF
{
    /// <summary>
    /// Interaction logic for WindowForm2.xaml
    /// </summary>
    public partial class WindowForm2 : Window
    {
        //create singleton patern
        private static WindowForm2 instance;
        public static WindowForm2 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowForm2();
                }
                return instance;
            }
        }

        //create property for label
        public string LabelText
        {
            get { return label1.Content.ToString(); }
            set { label1.Content = value; }
        }

        private WindowForm2()
        {
            InitializeComponent();
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = 0;
        }
        
        
        
    }
}
