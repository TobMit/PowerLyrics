﻿using System;
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
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Vyska.Content = "Vyska: " + e.NewSize.Height.ToString();
            Sirka.Content = "Sirka: " + e.NewSize.Width.ToString();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.myDataContext.closeWindow();
            base.OnClosed(e);
        }
    }
}