using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLyrics.Core
{
    public enum LyricType
    {
        Verse,
        Chorus,
        Bridge,
        Undefined
    }

    public enum SlideType
    {
        Slide,
        Divider
    }

    class constants
    {
        public const int FONT_SIZE = 25;
        public static string DEFAULT_TEXT = "Mládežka " + DateTime.Now.ToString("dd.MM.yyyy");
    }
}
