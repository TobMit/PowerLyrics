using System;
using System.Collections.Generic;
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

        public void saveSong(SongModel openedSongModel)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PowerLyrics (*.pwly)|*.pwly";
            if (saveFileDialog.ShowDialog() == true)
            {
                writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate));
                writer.Write(constants.MAGICNUMBER_FILE); // magic number (PWLY)
                writer.Write(constants.MAGICNUMBER_SONG); // magic number (PWLY)
                writer.Write(openedSongModel.number);
                writer.Write(openedSongModel.name);
                this.writeLyricModel(openedSongModel.LyricModels);
                writer.Close();

                /*BinaryReader Reader = new BinaryReader(File.Open(saveFileDialog.FileName, FileMode.Open));
                if (Reader.ReadString().Equals("PWLY"))
                {
                    Debug.WriteLine(Reader.ReadString());
                    Debug.WriteLine(Reader.ReadInt32());
                }
                //Debug.WriteLine(Reader.ReadString());
                Reader.Close();*/
            }
        }

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