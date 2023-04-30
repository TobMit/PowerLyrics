using System;
using System.Windows;

namespace PowerLyrics.Windows;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SizeChanged += OnWindowSizeChanged;
    }

    protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        //Vyska.Content = "Vyska: " + e.NewSize.Height.ToString();
        //Sirka.Content = "Sirka: " + e.NewSize.Width.ToString();
    }

    protected override void OnClosed(EventArgs e)
    {
        myDataContext.closeWindow();
        base.OnClosed(e);
    }
}