using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PowerLyrics.Core;
using PowerLyrics.MVVM.Model.SlideContentModels;
using PowerLyrics.Windows;
using WpfScreenHelper;

namespace PowerLyrics.MVVM.View;

/// <summary>
///     Interaction logic for LyricViewTemplateText.xaml Toto je zobrazovaný kontent
/// </summary>
public partial class LyricViewTemplateText : LyricViewTemplate
{
    public LyricViewTemplateText()
    {
        InitializeComponent();
        if (Screen.AllScreens.ToArray().Length > 1)
        {
            var dimensions = Screen.AllScreens.ToArray()[1].WorkingArea;
            var offset = dimensions.Size.Width - dimensions.Size.Width > 900 ? 200 : 50;
            this.TextBlock.Width = dimensions.Size.Width - offset;
            this.Grid.Width = dimensions.Size.Width;
            this.Grid.Height = dimensions.Size.Height;
        }
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

            InitializeComponent();
            if (Screen.AllScreens.ToArray().Length > 1)
            {
                var dimensions = Screen.AllScreens.ToArray()[1].WorkingArea;
                var offset = dimensions.Size.Width - dimensions.Size.Width > 900 ? 200 : 50;
                this.TextBlock.Width = dimensions.Size.Width - offset;
                this.Grid.Width = dimensions.Size.Width;
                this.Grid.Height = dimensions.Size.Height;
            }
        }
    }

    public LyricViewTemplateText(LyricModel lyric)
    {
        InitializeComponent();
        text = lyric.text;
        fontSize = lyric.fontSize;
        fontFamily = lyric.fontFamily;
        textAligment = lyric.textAligment;

        InitializeComponent();
        if (Screen.AllScreens.ToArray().Length > 1)
        {
            var dimensions = Screen.AllScreens.ToArray()[1].WorkingArea;
            var offset = dimensions.Size.Width - dimensions.Size.Width > 900 ? 200 : 50;
            this.TextBlock.Width = dimensions.Size.Width - offset;
            this.Grid.Width = dimensions.Size.Width;
            this.Grid.Height = dimensions.Size.Height;
        }
    }

    public LyricViewTemplateText(string text)
    {
        InitializeComponent();
        this.text = text;
        this.fontSize = constants.FONT_SIZE;
        this.fontFamily = constants.DEFAULT_FONT_FAMILY;
        this.textAligment = constants.DEFAULT_TEXT_ALIGNMENT;

        InitializeComponent();
        if (Screen.AllScreens.ToArray().Length > 1)
        {
            var dimensions = Screen.AllScreens.ToArray()[1].WorkingArea;
            var offset = dimensions.Size.Width - dimensions.Size.Width > 900 ? 200 : 50;
            this.TextBlock.Width = dimensions.Size.Width - offset;
            this.Grid.Width = dimensions.Size.Width;
            this.Grid.Height = dimensions.Size.Height;
        }
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

    public override LyricViewTemplate Clone()
    {
        return new LyricViewTemplateText(this);
    }

    public override SlideContentType GetType()
    {
        return SlideContentType.Text;
    }
}