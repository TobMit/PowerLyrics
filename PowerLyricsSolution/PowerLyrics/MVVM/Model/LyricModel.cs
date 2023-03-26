using System.Windows;
using System.Windows.Media;
using PowerLyrics.Core;

namespace PowerLyrics.MVVM.Model
{
    public class LyricModel
    {
        public string text { get; set; }
        public int fontSize { get; set; }
        public FontFamily fontFamily { get; set; }
        public LyricType LyricType { get; set; }
        public TextAlignment textAligment { get; set; }
        public int serialNuber { get; set; } // in case there are more verses or bridges...

        public LyricModel()
        {
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
            }
        }
    }
}
