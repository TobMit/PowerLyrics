﻿using System;
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

        public List<LyricModel> parseLyric(SongModel songModel)
        {
            var tmpSlides = new List<LyricModel>();
            int verse = 0;
            int bridge = 0;
            int chorus = 0;

            for (int i = 0; i < songModel.lyricTypeQueue.Count; i++)
            {
                var tmp = new LyricModel();
                switch (songModel.lyricTypeQueue[i])
                {
                    case LyricType.Verse:
                        tmp.text = songModel.verse[verse].ToString();
                        tmp.LyricType = LyricType.Verse;
                        tmp.serialNuber = verse + 1; // iba kvoli vypisu
                        verse++;
                        break;
                    case LyricType.Chorus:
                        tmp.text = songModel.chorus[chorus].ToString();
                        tmp.LyricType = LyricType.Chorus;
                        tmp.serialNuber = chorus + 1;
                        chorus++;
                        break;
                    case LyricType.Bridge:
                        tmp.text = songModel.bridge[bridge].ToString();
                        tmp.LyricType = LyricType.Bridge;
                        tmp.serialNuber = bridge + 1;
                        bridge++;
                        break;
                }
                tmp.fontSize= constants.FONT_SIZE;
                tmp.fontFamily = constants.FOUNT_FAMILY;
                tmpSlides.Add(tmp);
            }
            return tmpSlides;
        }
        public ObservableCollection<Slide> getSlidesFromOpenSong(List<LyricModel> song)
        {
            ObservableCollection<Slide> tmp = new ObservableCollection<Slide>();
            int id = 0;
            LyricType oldType = LyricType.Undefined;
            int oldSerialNumber = 1;
            foreach (var item in song)
            {
                /*if (oldType == LyricType.Undefined || oldType != item.LyricType || oldSerialNumber != item.serialNuber)
                {
                    oldType = item.LyricType;
                    oldSerialNumber = item.serialNuber;
                    tmp.Add(new Slide()
                    {
                        SlideType = SlideType.Divider,
                        dividerText = item.serialNuber + ". " + item.LyricType.ToString(),
                    });


                    id++;
                }*/
                Slide slide = new Slide();
                slide.UserControl = new LyricViewTemplate1(item);
                slide.id = id;
                slide.SlideType = SlideType.Slide;
                slide.LyricType = item.LyricType;
                slide.isSelected = false;
                tmp.Add(slide);
                id++;
            }
            return tmp;
        }
    }
}
