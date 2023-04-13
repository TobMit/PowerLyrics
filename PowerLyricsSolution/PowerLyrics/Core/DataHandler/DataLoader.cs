using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

    // ak je nacitany súbor môjho formátu
    private bool myFileType;
    private ObservableCollection<SongModel> playlist;
    private BinaryReader reader;
    private SongModel songModel;
    private readonly TextParser.TextParser textParser;

    public DataLoader()
    {
        _regex = new Regex(@"\s+");
        textParser = new TextParser.TextParser();
        openedFileType = FileType.undefined;
    }

    public FileType openedFileType { get; set; }

    /**
     * Načíta pieseň z file do string
     */
    private string loadSong(string path)
    {
        var loadedSong = File.ReadAllText(path);
        return _regex.Replace(loadedSong, " ");
    }

    /**
     * Získa piesne pre knižnicu
     */
    public ObservableCollection<SongModel> getSongs()
    {
        var songs = new ObservableCollection<SongModel>();
        try
        {
            paths = Directory.GetFiles(Path.GetDirectoryName(
                                           Assembly.GetEntryAssembly().Location) +
                                       "\\Songs"); // toto vyriešilo ten bug ktorý me nechcel dovoliť bindowanie v xaml preview
            // problém bol v tom že sa aplikácia mohla spúšťať z rôznych lokácii keď sa debugova a ked sa vyvýjala, to spôsobovalo že nie vždy exsitoval tento priečino
            // táto chyba sa prejavila až keď som sa snažil spustiť aplikáciu pomocou súboru

            foreach (var path in paths)
            {
                var loadedSong = loadSong(path);
                var tmpSongModel = processSong(loadedSong);
                tmpSongModel.name = getName(path);
                tmpSongModel.number = getSongNumber(path);
                tmpSongModel.LyricModels = textParser.parseLyric(tmpSongModel);
                songs.Add(tmpSongModel);
            }

            //sort songs by id
            songs = new ObservableCollection<SongModel>(songs.OrderBy(x => x.number));
            for (var i = 0; i < songs.Count; i++) songs[i].id = i;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "toto je test", MessageBoxButton.OK,
                MessageBoxImage.Information);
            throw;
        }

        return songs;
    }

    /**
     * zísak meno z názvu súboru
     */
    private string getName(string song)
    {
        var splitedSong = getFileName(song).Split(".");
        if (splitedSong.Length > 2)
            return splitedSong[1].Remove(0, 1);
        return splitedSong[0];
    }

    /**
     * získa číslo piense z názvu súboru
     */
    private int getSongNumber(string song)
    {
        var splitedSong = getFileName(song).Split(".");
        if (splitedSong.Length > 2)
            return int.Parse(splitedSong[0]);
        return 0;
    }

    /**
     * získa celé mno súboru
     */
    private string getFileName(string path)
    {
        var splitedPath = path.Split(@"\");
        return splitedPath[splitedPath.Length - 1];
    }

    /**
     * state machina pre spracovanie raw textu
     */
    private SongModel processSong(string song)
    {
        var tmpSongModel = new SongModel();
        var splitedSong = song.Split(" ");
        var stat = LyricType.Verse;

        var builder = new StringBuilder();

        //state machine for lyric parsing
        foreach (var word in splitedSong)
            //chceking for type ([V1]...)
            if (word.Contains("[") || word.Contains("]"))
            {
                if (builder.Length != 0)
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

                builder.Clear();

                if (word.Contains("C"))
                    stat = LyricType.Chorus;
                else if (word.Contains("B"))
                    stat = LyricType.Bridge;
                else
                    stat = LyricType.Verse;

                tmpSongModel.lyricTypeQueue.Add(stat);
            }
            else
            {
                if (builder.Length == 0)
                    builder.Append(word);
                else
                    builder.Append(" " + word);
            }

        //aby sa aj posledna cast piesne nacitala do pamete
        if (builder.Length != 0)
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

        return tmpSongModel;
    }

    /**
     * Načíta pieseň s otovrením dialógového okna
     */
    public void loadFile()
    {
        openedFileType = FileType.undefined;
        var opneFileDialog = new OpenFileDialog();
        opneFileDialog.Filter = "PowerLyric (*.pwly)|*.pwly|Text files (*.txt)|*.txt";
        if (opneFileDialog.ShowDialog() == true) loadFileStartUp(opneFileDialog.FileName);
    }

    /**
     * načíta pieseň alebo playlist pri start-up
     */
    public void loadFileStartUp(string path)
    {
        var splitedPath = path.Split(@".");
        myFileType = !splitedPath[splitedPath.Length - 1].Contains("txt");

        if (myFileType)
        {
            processMyFile(path);
        }
        else
        {
            openedFileType = FileType.Song;
            songModel = processSong(loadSong(path));
            songModel.name = getName(path);
            songModel.number = getSongNumber(path);
            songModel.LyricModels = textParser.parseLyric(songModel);
        }
    }

    /**
     * Správne načíta zadaný súbor.
     */
    private void processMyFile(string fileName)
    {
        reader = new BinaryReader(File.Open(fileName, FileMode.Open));
        if (reader.ReadInt32() == constants.MAGICNUMBER_FILE)
        {
            openedFileType = reader.ReadInt32() == constants.MAGICNUMBER_SONG ? FileType.Song : FileType.PlayList;
            if (openedFileType == FileType.Song)
                processMyFileSong();
            else
                processMyFilePlayList();
        }

        //Debug.WriteLine(Reader.ReadString());
        reader.Close();
    }

    /**
     * Načíta pieseň z .ppwly súboru
     */
    private void processMyFileSong()
    {
        songModel = new SongModel();
        songModel.number = reader.ReadInt32();
        songModel.name = reader.ReadString();
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var tmp = new LyricModel();
            tmp.text = reader.ReadString();
            tmp.fontSize = reader.ReadInt32();
            tmp.fontFamily = new FontFamily(reader.ReadString());
            tmp.LyricType = (LyricType)Enum.Parse(typeof(LyricType), reader.ReadString());
            tmp.textAligment = (TextAlignment)Enum.Parse(typeof(TextAlignment), reader.ReadString());
            tmp.serialNuber = reader.ReadInt32();
            songModel.LyricModels.Add(tmp);
        }
    }

    /**
     * Načíta playlist z .pwly súboru
     */
    private void processMyFilePlayList()
    {
        playlist = new ObservableCollection<SongModel>();
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var tmp = new SongModel();
            tmp.id = i;
            tmp.number = reader.ReadInt32();
            tmp.name = reader.ReadString();
            var count2 = reader.ReadInt32();
            for (var j = 0; j < count2; j++)
            {
                var tmp2 = new LyricModel();
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

    /**
     * Vráti aktuálne načítanú songModel
     */
    public SongModel getSongModel()
    {
        return new SongModel(songModel);
    }

    /**
     * Vráti akutálne načítaný playlist
     */
    public ObservableCollection<SongModel> getPlaylist()
    {
        return new ObservableCollection<SongModel>(playlist);
    }
}