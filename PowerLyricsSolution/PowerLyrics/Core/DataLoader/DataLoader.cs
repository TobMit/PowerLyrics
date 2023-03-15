using System;
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
    public DataLoader()
    {
        _regex = new Regex(@"\s+");
        paths = Directory.GetFiles("Songs");
    }

    private string loadSong(string path)
    {
        string loadedSong = File.ReadAllText(path);
        return _regex.Replace(loadedSong, " ");
    }

    public ObservableCollection<Song> getSongs()
    {
        ObservableCollection<Song> songs = new ObservableCollection<Song>();
        foreach (string path in paths)
        {
            string loadedSong = this.loadSong(path);
            songs.Add(this.processSong(loadedSong));
        }

        return songs;
    }

    private Song processSong(string song)
    {
        Song tmpSong = new Song();
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
                            tmpSong.bridge.Add(builder.ToString());
                            break;
                        case LyricType.Verse:
                            tmpSong.verse.Add(builder.ToString());
                            break;
                        case LyricType.Chorus:
                            tmpSong.chorus.Add(builder.ToString());
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
                    tmpSong.bridge.Add(builder.ToString());
                    break;
                case LyricType.Verse:
                    tmpSong.verse.Add(builder.ToString());
                    break;
                case LyricType.Chorus:
                    tmpSong.chorus.Add(builder.ToString());
                    break;
            }
        }
        
        return tmpSong;
    }
}