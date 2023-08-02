using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PowerLyrics.MVVM.Model;

namespace PowerLyrics.MVVM.View;

/// <summary>
///     Interaction logic for LyricViewTemplateText.xaml Toto je zobrazovaný kontent
/// </summary>
public partial class LyricViewTemplateText : LyricViewTemplate
{
    public LyricViewTemplateText()
    {
        InitializeComponent();
    }

    public LyricViewTemplateText(LyricViewTemplateText copy)
    {
        if (copy != null)
        {
            InitializeComponent();
            text = copy.text;
            fontSize = copy.fontSize;
            if (copy.fontFamily != null) fontFamily = copy.fontFamily;
            textAligment = copy.textAligment;
        }
    }

    public LyricViewTemplateText(LyricModel lyric)
    {
        InitializeComponent();
        text = lyric.text;
        fontSize = lyric.fontSize;
        fontFamily = lyric.fontFamily;
        textAligment = lyric.textAligment;
    }

    public LyricViewTemplateText(string text)
    {
        InitializeComponent();
        this.text = text;
    }

    public string text
    {
        get => TextBlock.Text;
        set => TextBlock.Text = value;
    }

    public FontFamily fontFamily
    {
        get => TextBlock.FontFamily;
        set => TextBlock.FontFamily = value;
    }

    public double fontSize
    {
        get => TextBlock.FontSize;
        set => TextBlock.FontSize = value;
    }

    public TextAlignment textAligment
    {
        get => TextBlock.TextAlignment;
        set => TextBlock.TextAlignment = value;
    }

    public override object Clone()
    {
        return new LyricViewTemplateText(this);
    }
}