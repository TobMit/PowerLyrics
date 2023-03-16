using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.Core.TextParser
{
    internal class TextParser
    {
        public TextParser()
        {
            
        }

        public List<LyricModel> parseLyric(Song song)
        {
            var tmpSlides = new List<LyricModel>();
            for (int i = 0; i < 50; i++)
            {
                var tmp = new LyricModel();
                tmp.text = "test" + i;
                tmp.fontSize = 50;
                tmp.LyricType = i < 20 ? LyricType.Verse : LyricType.Chorus; 
                tmpSlides.Add(tmp);
            }
            return tmpSlides;
        }
    }
}
