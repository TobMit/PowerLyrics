using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using PowerLyrics.MVVM.Model;

namespace PowerLyrics.Core.DataHandler;

internal class DataSaver
{
    private BinaryWriter writer;

    /**
         * Uloží pieseň do súboru pomocou dialógového okna
         */
    public void saveSong(SongModel openedSongModel)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "PowerLyrics (*.pwly)|*.pwly";
        saveFileDialog.FileName = openedSongModel.number + ". " + openedSongModel.name;
        if (saveFileDialog.ShowDialog() == true)
        {
            writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
            writer.Write(constants.MAGICNUMBER_FILE); // magic number (PWLY)
            writer.Write(constants.MAGICNUMBER_SONG); // magic number (PWLY)
            writer.Write(openedSongModel.number);
            writer.Write(openedSongModel.name);
            writeLyricModel(openedSongModel.LyricModels);
            writer.Close();
        }
    }

    /**
         * Uloží playlist do súboru pomocou dialógového okna
         */
    public void savePlaylist(ObservableCollection<SongModel> listOfSongModels)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "PowerLyrics (*.pwly)|*.pwly";
        saveFileDialog.FileName = "playlist";
        if (saveFileDialog.ShowDialog() == true)
        {
            writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
            writer.Write(constants.MAGICNUMBER_FILE); // magic number (PWLY)
            writer.Write(constants.MAGICNUMBER_PLAYLIST); // magic number (PWLY)
            writer.Write(listOfSongModels.Count);
            foreach (var songModel in listOfSongModels)
            {
                writer.Write(songModel.number);
                writer.Write(songModel.name);
                writeLyricModel(songModel.LyricModels);
            }

            writer.Close();
        }
    }

    /**
         * Zapíše do súboru LyricModel
         */
    private void writeLyricModel(List<LyricModel> lyricModels)
    {
        writer.Write(lyricModels.Count);
        foreach (var lyricModel in lyricModels)
        {
            writer.Write(lyricModel.text);
            writer.Write(lyricModel.fontSize);
            writer.Write(lyricModel.fontFamily.ToString());
            writer.Write(lyricModel.LyricType.ToString());
            writer.Write(lyricModel.textAligment.ToString());
            writer.Write(lyricModel.serialNuber);
        }
    }
}