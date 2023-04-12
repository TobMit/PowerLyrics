using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using PowerLyrics.MVVM.Model;

namespace PowerLyrics.Core.DataHandler
{
    internal class DataSaver
    {
        private BinaryWriter writer;

        public DataSaver()
        {
        }
        /**
         * Uloží pieseň do súboru pomocou dialógového okna
         */
        public void saveSong(SongModel openedSongModel)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PowerLyrics (*.pwly)|*.pwly";
            saveFileDialog.FileName = openedSongModel.number + ". " + openedSongModel.name;
            if (saveFileDialog.ShowDialog() == true)
            {
                writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
                writer.Write(constants.MAGICNUMBER_FILE); // magic number (PWLY)
                writer.Write(constants.MAGICNUMBER_SONG); // magic number (PWLY)
                writer.Write(openedSongModel.number);
                writer.Write(openedSongModel.name);
                this.writeLyricModel(openedSongModel.LyricModels);
                writer.Close();
            }
        }
        /**
         * Uloží playlist do súboru pomocou dialógového okna
         */
        public void savePlaylist(ObservableCollection<SongModel> listOfSongModels)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PowerLyrics (*.pwly)|*.pwly";
            saveFileDialog.FileName = "playlist";
            if (saveFileDialog.ShowDialog() == true)
            {
                writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
                writer.Write(constants.MAGICNUMBER_FILE); // magic number (PWLY)
                writer.Write(constants.MAGICNUMBER_PLAYLIST); // magic number (PWLY)
                writer.Write(listOfSongModels.Count);
                foreach (SongModel songModel in listOfSongModels)
                {
                    writer.Write(songModel.number);
                    writer.Write(songModel.name);
                    this.writeLyricModel(songModel.LyricModels);
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
            foreach (LyricModel lyricModel in lyricModels)
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
}