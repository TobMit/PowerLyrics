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

namespace TwoFromWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isClicked = false;
        private WindowForm2 windowForm2 = WindowForm2.Instance;
        private int tmpValue=0;


        //this is the constructor
        public MainWindow()
        {
            InitializeComponent();
            Console.Write("Hello world");
        }
        

        //create function for button click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!isClicked)
            {
                //show new window
                windowForm2.Show();
                isClicked = true;
            }
            else
            {
                windowForm2.LabelText = tmpValue.ToString();
                tmpValue += int.Parse(((Button)sender).Tag.ToString());
            }
            
        }

    }
}
