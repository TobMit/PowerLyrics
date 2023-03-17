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
            int verse = 0;
            int bridge = 0;
            int chorus = 0;

            for (int i = 0; i < song.lyricTypeQueue.Count; i++)
            {
                var tmp = new LyricModel();
                switch (song.lyricTypeQueue[i])
                {
                    case LyricType.Verse:
                        tmp.text = song.verse[verse].ToString();
                        tmp.fontSize = constants.FONT_SIZE;
                        tmp.LyricType = LyricType.Verse;
                        tmp.serialNuber = verse + 1; // iba kvoli vypisu
                        verse++;
                        tmpSlides.Add(tmp);
                        break;
                    case LyricType.Chorus:
                        tmp.text = song.chorus[chorus].ToString();
                        tmp.fontSize = constants.FONT_SIZE;
                        tmp.LyricType = LyricType.Chorus;
                        tmp.serialNuber = chorus + 1;
                        chorus++;
                        tmpSlides.Add(tmp);
                        break;
                    case LyricType.Bridge:
                        tmp.text = song.bridge[bridge].ToString();
                        tmp.fontSize = constants.FONT_SIZE;
                        tmp.LyricType = LyricType.Bridge;
                        tmp.serialNuber = bridge + 1;
                        bridge++;
                        tmpSlides.Add(tmp);
                        break;

                }
            }
            /* old parser
            for (int i = 0; i < song.verse.Count; i++)
            {
                var tmp = new LyricModel();
                tmp.text = song.verse[i].ToString();
                tmp.fontSize = 20;
                tmp.LyricType = LyricType.Verse;
                tmp.serialNuber = i + 1;
                tmpSlides.Add(tmp);
            }
            for (int i = 0; i < song.chorus.Count; i++)
            {
                var tmp = new LyricModel();
                tmp.text = song.chorus[i].ToString();
                tmp.fontSize = 20;
                tmp.LyricType = LyricType.Chorus;
                tmp.serialNuber = i + 1;
                tmpSlides.Add(tmp);
            }
            for (int i = 0; i < song.bridge.Count; i++)
            {
                var tmp = new LyricModel();
                tmp.text = song.bridge[i].ToString();
                tmp.fontSize = 20;
                tmp.LyricType = LyricType.Bridge;
                tmp.serialNuber = i + 1;
                tmpSlides.Add(tmp);
            }
            */
            return tmpSlides;
        }
    }
}
