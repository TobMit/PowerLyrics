﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PowerLyrics.MVVM.Model;

namespace PowerLyrics.Core.DataLoader;

public class DataLoader
{
    private static Regex _regex;
    private static string[] paths;
    private TextParser.TextParser textParser;
    public DataLoader()
    {
        _regex = new Regex(@"\s+");
        paths = Directory.GetFiles("Songs");
        textParser = new TextParser.TextParser();
    }

    private string loadSong(string path)
    {
        string loadedSong = File.ReadAllText(path);
        return _regex.Replace(loadedSong, " ");
    }

    public ObservableCollection<SongModel> getSongs()
    {
        ObservableCollection<SongModel> songs = new ObservableCollection<SongModel>();
        foreach (string path in paths)
        {
            string loadedSong = this.loadSong(path);
            SongModel tmpSongModel = this.processSong(loadedSong);
            string[] splitedPath = path.Split(@"\");
            //z cesty vyberiem meno suboru
            string rawName = splitedPath[splitedPath.Length - 1];
            //rozdelim 
            string[] splitedRawName = rawName.Split(".");
            tmpSongModel.number = Int32.Parse(splitedRawName[0]);
            tmpSongModel.name = splitedRawName[1].Remove(0,1);
            tmpSongModel.LyricModels = textParser.parseLyric(tmpSongModel);
            songs.Add(tmpSongModel);
        }

        //sort songs by id
        songs = new ObservableCollection<SongModel>(songs.OrderBy(x => x.number));
        for (int i = 0; i < songs.Count; i++)
        {
            songs[i].id = i;
        }

        return songs;
    }

    private SongModel processSong(string song)
    {
        SongModel tmpSongModel = new SongModel();
        string[] splitedSong = song.Split(" ");
        LyricType stat = LyricType.Verse;

        StringBuilder builder = new StringBuilder();
        
        //state machine for lyric parsing
        foreach (string word in splitedSong)
        {
            //chceking for type ([V1]...)
            if (word.Contains("[") || word.Contains("]"))
            {
                if (builder.Length != 0)
                {
                    switch (stat)
                    {
                        case LyricType.Bridge:
                            tmpSongModel.bridge.Add(builder.ToString());
                            break;
                        case LyricType.Verse:
                            tmpSongModel.verse.Add(builder.ToString());
                            break;
                        case LyricType.Chorus:
                            tmpSongModel.chorus.Add(builder.ToString());
                            break;
                    }
                }

                builder.Clear();
                
                if (word.Contains("C"))
                {
                    stat = LyricType.Chorus;
                }
                else if (word.Contains("B"))
                {
                    stat = LyricType.Bridge;
                }
                else
                {
                    stat = LyricType.Verse;
                }
                tmpSongModel.lyricTypeQueue.Add(stat);
            }
            else
            {
                if (builder.Length == 0)
                {
                    builder.Append(word);
                }
                else
                {
                    builder.Append(" " + word);
                }
            }
        }
        
        //aby sa aj posledna cast piesne nacitala do pamete
        if (builder.Length != 0)
        {
            switch (stat)
            {
                case LyricType.Bridge:
                    tmpSongModel.bridge.Add(builder.ToString());
                    break;
                case LyricType.Verse:
                    tmpSongModel.verse.Add(builder.ToString());
                    break;
                case LyricType.Chorus:
                    tmpSongModel.chorus.Add(builder.ToString());
                    break;
            }
        }
        
        return tmpSongModel;
    }
}