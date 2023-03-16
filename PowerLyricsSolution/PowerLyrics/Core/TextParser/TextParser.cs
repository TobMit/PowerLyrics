using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<LyricModel> parseLyric(Song song)
        {
            var tmpSlides = new ObservableCollection<LyricModel>();
            for (int i = 0; i < 50; i++)
            {
                var tmp = new LyricModel();
                tmp.text = "test" + i;
                tmp.fontSize = 50;
                tmp.id = i;
                tmp.LyricType = LyricType.Verse;
                var tmpTemplate = new LyricViewTemplate1(); // zatial takto natvrdo dizajn
                tmpTemplate.text = tmp.text;
                tmpTemplate.fontSize = tmp.fontSize;
                tmp.UserControlContent = tmpTemplate;
                tmpSlides.Add(tmp);
            }
            return tmpSlides;
        }
    }
}
