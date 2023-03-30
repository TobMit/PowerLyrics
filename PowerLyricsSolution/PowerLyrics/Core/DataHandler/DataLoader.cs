using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using PowerLyrics.MVVM.Model;

namespace PowerLyrics.Core.DataHandler;

public class DataLoader
{
    private static Regex _regex;
    private static string[] paths;
    private TextParser.TextParser textParser;
    private BinaryReader reader;

    public FileType openedFileType { get; set; }

    // ak je nacitany súbor môjho formátu
    bool myFileType = false;
    private SongModel songModel;
    private ObservableCollection<SongModel> playlist;

    public DataLoader()
    {
        _regex = new Regex(@"\s+");
        paths = Directory.GetFiles("Songs");
        textParser = new TextParser.TextParser();
        openedFileType = FileType.undefined;
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
            tmpSongModel.name = getName(path);
            tmpSongModel.number = getSongNumber(path);
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

    private string getName(string song)
    {
        string[] splitedSong = getFileName(song).Split(".");
        if (splitedSong.Length > 2)
        {
            return splitedSong[1].Remove(0, 1);
        }
        else
        {
            return splitedSong[0];
        }
    }

    private int getSongNumber(string song)
    {
        string[] splitedSong = getFileName(song).Split(".");
        if (splitedSong.Length > 2)
        {
            return Int32.Parse(splitedSong[0]);
        }
        else
        {
            return 0;
        }
    }

    private string getFileName(string path)
    {
        string[] splitedPath = path.Split(@"\");
        return splitedPath[splitedPath.Length - 1];
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

    public void loadFile()
    {
        OpenFileDialog opneFileDialog = new OpenFileDialog();
        opneFileDialog.Filter = "Text files (*.txt)|*.txt|PowerLyric (*.pwly)|*.pwly";
        if (opneFileDialog.ShowDialog() == true)
        {
            string[] splitedPath = opneFileDialog.FileName.Split(@".");
            myFileType = !splitedPath[splitedPath.Length - 1].Contains("txt");

            if (myFileType)
            {
                this.processMyFile(opneFileDialog.FileName);
            }
            else
            {
                openedFileType = FileType.Song;
                songModel = this.processSong(loadSong(opneFileDialog.FileName));
                songModel.name = getName(opneFileDialog.FileName);
                songModel.number = getSongNumber(opneFileDialog.FileName);
                songModel.LyricModels = textParser.parseLyric(songModel);
            }
        }
    }

    private void processMyFile(string fileName)
    {
        reader = new BinaryReader(File.Open(fileName, FileMode.Open));
        if (reader.ReadInt32() == constants.MAGICNUMBER_FILE)
        {
            openedFileType = reader.ReadInt32() == constants.MAGICNUMBER_SONG ? FileType.Song : FileType.PlayList;
            if (openedFileType == FileType.Song)
            {
                processMyFileSong();
            }
            else
            {
                processMyFilePlayList();
            }
        }

        //Debug.WriteLine(Reader.ReadString());
        reader.Close();
    }

    private void processMyFileSong()
    {
        songModel = new SongModel();
        songModel.number = reader.ReadInt32();
        songModel.name = reader.ReadString();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            LyricModel tmp = new LyricModel();
            tmp.text = reader.ReadString();
            tmp.fontSize = reader.ReadInt32();
            tmp.fontFamily = new FontFamily(reader.ReadString());
            tmp.LyricType = (LyricType)Enum.Parse(typeof(LyricType), reader.ReadString());
            tmp.textAligment = (TextAlignment)Enum.Parse(typeof(TextAlignment), reader.ReadString());
            tmp.serialNuber = reader.ReadInt32();
            songModel.LyricModels.Add(tmp);
        }
    }

    private void processMyFilePlayList()
    {
        playlist = new ObservableCollection<SongModel>();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            SongModel tmp = new SongModel();
            tmp.id = i;
            tmp.number = reader.ReadInt32();
            tmp.name = reader.ReadString();
            int count2 = reader.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                LyricModel tmp2 = new LyricModel();
                tmp2.text = reader.ReadString();
                tmp2.fontSize = reader.ReadInt32();
                tmp2.fontFamily = new FontFamily(reader.ReadString());
                tmp2.LyricType = (LyricType)Enum.Parse(typeof(LyricType), reader.ReadString());
                tmp2.textAligment = (TextAlignment)Enum.Parse(typeof(TextAlignment), reader.ReadString());
                tmp2.serialNuber = reader.ReadInt32();
                tmp.LyricModels.Add(tmp2);
            }

            playlist.Add(tmp);
        }
    }


    public SongModel getSongModel()
    {
        return new SongModel(songModel);
    }

    public ObservableCollection<SongModel> getPlaylist()
    {
        return new ObservableCollection<SongModel>(playlist);
    }
}