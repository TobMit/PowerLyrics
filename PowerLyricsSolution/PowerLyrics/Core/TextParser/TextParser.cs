using System.Collections.Generic;
using System.Collections.ObjectModel;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.Model.SlideContentModels;
using PowerLyrics.MVVM.View;

namespace PowerLyrics.Core.TextParser;

internal class TextParser
{
    /**
         * Rozdelý text z listov do LyricType
         */
    public List<ContentModel> parseLyric(SongModel songModel)
    {
        var tmpSlides = new List<ContentModel>();
        var verse = 0;
        var bridge = 0;
        var chorus = 0;

        for (var i = 0; i < songModel.lyricTypeQueue.Count; i++)
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

            tmp.fontSize = constants.FONT_SIZE;
            tmp.fontFamily = constants.DEFAULT_FONT_FAMILY;
            tmp.textAligment = constants.DEFAULT_TEXT_ALIGNMENT;
            tmpSlides.Add(tmp);
        }

        return tmpSlides;
    }

    /**
         * Vytvorí z listu LyricModel kolekciu slide pre pieseň
         */
    public ObservableCollection<Slide> getSlidesFromOpenSong(List<ContentModel> song)
    {
        var tmp = new ObservableCollection<Slide>();
        var id = 0;
        var oldType = LyricType.Undefined;
        var oldSerialNumber = 1;
        foreach (var item in song)
        {
            var slide = getSlideFromLyricModel(item);
            slide.id = id;
            tmp.Add(slide);
            id++;
        }

        return tmp;
    }

    /**
         * Vytvorí z listu LyricModel kolekciu slide pre playlist
         */
    public ObservableCollection<Slide> getSlidesFromOpenSong(ObservableCollection<SongModel> listOfSong,
        List<SlideSongIndexingModel> slideSongIndexing)
    {
        var tmp = new ObservableCollection<Slide>();
        var id = 0;
        foreach (var song in listOfSong)
        {
            // display name of song
            var tmpSlide = new Slide();
            tmpSlide.SlideType = SlideType.Divider;
            tmpSlide.labelText = song.number + " " + song.name;
            tmpSlide.id = id;
            tmp.Add(tmpSlide);
            id++;

            //set index of first slide of song
            var tmpSongIndexingModel = new SlideSongIndexingModel();
            tmpSongIndexingModel.indexOfFirstSlide = id;


            // set name of song
            var tmpLyricModel = new LyricModel();
            tmpLyricModel.text = song.name;
            tmpLyricModel.LyricType = LyricType.Verse;
            tmpLyricModel.serialNuber = 0;
            //tmpLyricModel.text = "";
            tmpLyricModel.LyricType = LyricType.Undefined;
            var tmpSlideSongName = getSlideFromLyricModel(tmpLyricModel);
            tmpSlideSongName.id = id;
            tmp.Add(tmpSlideSongName);
            id++;

            // parse song and add slides
            foreach (var lyricModel in song.ContentModels)
            {
                var slide = getSlideFromLyricModel(lyricModel);
                slide.id = id;
                tmp.Add(slide);
                id++;
            }

            tmpSongIndexingModel.indexOfLastSlide = id - 1; // pretože sa mi čislo posledného slide zvyšilo v cykle
            slideSongIndexing.Add(tmpSongIndexingModel);
        }

        return tmp;
    }

    /**
         * Vytvorí Slide z LyricModel
         */
    public Slide getSlideFromLyricModel(ContentModel contentModel)
    {
        var slide = new Slide();
        if (contentModel.slideContentType == SlideContentType.Text)
        {
            slide.UserControl = new LyricViewTemplateText((LyricModel)contentModel);
            slide.SlideType = SlideType.Slide;
            slide.LyricType = contentModel.LyricType;
            slide.isSelected = false;
            slide.labelText = contentModel.LyricType == LyricType.Undefined
                ? "Name"
                : contentModel.LyricType + " " + contentModel.serialNuber;
        }
        else
        {
            //todo implement for video
            slide.UserControl = new LyricViewTemplateVideo((VideoModel)contentModel);
            slide.SlideType = SlideType.Slide;
            slide.LyricType = contentModel.LyricType;
            slide.isSelected = false;
            slide.labelText = contentModel.LyricType == LyricType.Undefined
                ? "Name"
                : contentModel.LyricType + " " + contentModel.serialNuber;
        }
        return slide;
    }
}