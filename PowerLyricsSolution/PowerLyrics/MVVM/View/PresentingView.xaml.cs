﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.MVVM.View;

public partial class PresentingView : UserControl
{
    public PresentingView()
    {
        InitializeComponent();
    }

    public PresentingViewModel getDataContext()
    {
        return this.presentingDataContext;
    }

    private void ListViewSlides_OnKeyUp(object sender, KeyEventArgs e)
    {
        this.presentingDataContext.key(e);
    }

}