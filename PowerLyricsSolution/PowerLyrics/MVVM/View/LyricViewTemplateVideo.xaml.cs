using PowerLyrics.MVVM.Model;
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

namespace PowerLyrics.MVVM.View
{
    /// <summary>
    /// Interaction logic for LyricViewTemplateVideo.xaml
    /// </summary>
    public partial class LyricViewTemplateVideo : LyricViewTemplate
    {
        public LyricViewTemplateVideo()
        {
            InitializeComponent();
        }

        public LyricViewTemplateVideo(LyricViewTemplateVideo copy)
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

        public LyricViewTemplateVideo(LyricModel lyric)
        {
            InitializeComponent();
            text = lyric.text;
            fontSize = lyric.fontSize;
            fontFamily = lyric.fontFamily;
            textAligment = lyric.textAligment;
        }

        public LyricViewTemplateVideo(string text)
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
            return new LyricViewTemplateVideo(this);
        }
    }
}
