using System.Windows;
using System.Windows.Media;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model.SlideContentModels;

/**
     * Poskytuje vešetky potrebné dáta pre vytvorenie slide
     */
public class LyricModel : ContentModel
{

    public LyricModel()
    {
        text = "";
        fontSize = constants.FONT_SIZE;
        fontFamily = constants.DEFAULT_FONT_FAMILY;
        LyricType = LyricType.Verse;
        textAligment = constants.DEFAULT_TEXT_ALIGNMENT;
        slideContentType = SlideContentType.Text;
        serialNuber = 0;
    }

    public LyricModel(LyricModel copy)
    {
        if (copy != null)
        {
            text = copy.text;
            fontSize = copy.fontSize;
            fontFamily = copy.fontFamily;
            LyricType = copy.LyricType;
            textAligment = copy.textAligment;
            serialNuber = copy.serialNuber;
            slideContentType = copy.slideContentType;
        }
    }

    public override ContentModel Clone()
    {
        return new LyricModel(this);
    }

    public string text { get; set; }
    public int fontSize { get; set; }
    public FontFamily fontFamily { get; set; }
    public TextAlignment textAligment { get; set; }
}