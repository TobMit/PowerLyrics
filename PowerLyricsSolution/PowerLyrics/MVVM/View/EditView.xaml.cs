﻿using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.MVVM.View;

public partial class EditView : UserControl
{
    private readonly Regex number;

    public EditView()
    {
        InitializeComponent();
        //input iba čísel
        number = new Regex("[^0-9]+");
    }

    public EditViewModel getDataContext()
    {
        return editDataContext;
    }

    /**
     * Umožní input iba čísel
     */
    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = number.IsMatch(e.Text);
    }
}