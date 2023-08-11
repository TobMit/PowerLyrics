using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.Model.SlideContentModels;

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
            writeLyricModel(openedSongModel.ContentModels);
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
                writeLyricModel(songModel.ContentModels);
            }

            writer.Close();
        }
    }

    /**
         * Zapíše do súboru LyricModel
         */
    private void writeLyricModel(List<ContentModel> contentModels)
    {
        writer.Write(contentModels.Count);
        foreach (var contentModel in contentModels)
        {
            if (contentModel.GetType() == typeof(LyricModel))
            {
                LyricModel lyricModel = (LyricModel) contentModel;
                writer.Write(lyricModel.text);
                writer.Write(lyricModel.fontSize);
                writer.Write(lyricModel.fontFamily.ToString());
                writer.Write(lyricModel.LyricType.ToString());
                writer.Write(lyricModel.textAligment.ToString());
                writer.Write(lyricModel.serialNuber);
            }
        }
    }
}