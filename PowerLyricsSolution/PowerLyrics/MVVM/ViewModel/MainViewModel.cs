using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerLyrics.Core;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;

namespace PowerLyrics.MVVM.ViewModel
{
    public class MainViewModel : ObservableObjects
    {

        private AudiencWindow audieceWindow;

        private object _lyricContent;
        public object LyricContent
        {
            get
            {
                return _lyricContent;
            }
            set
            {
                _lyricContent = value;
                audieceWindow.ContentControl.Content = new LyricViewTemplate1((LyricViewTemplate1)LyricContent);
                OnPropertyChanged();
            }
        }

        public RelayCommand test { get; set; }
        public RelayCommand test2 { get; set; }

        public MainViewModel()
        {
            audieceWindow = new AudiencWindow();
            audieceWindow.Show();
            LyricViewTemplate1 tesLyricViewTemplate1 = new LyricViewTemplate1();
            LyricContent = tesLyricViewTemplate1;

            test = new RelayCommand(o =>
            {
                LyricViewTemplate1 tesLyricViewTemplate2 = new LyricViewTemplate1();
                tesLyricViewTemplate2.Label.Content = "test";
                LyricContent = tesLyricViewTemplate2;
            });

            test2 = new RelayCommand(o =>
            {
                LyricViewTemplate1 tesLyricViewTemplate2 = new LyricViewTemplate1();
                tesLyricViewTemplate2.Label.Content = "Toto je test";
                LyricContent = tesLyricViewTemplate2;
            });

        }


        public void closeWindow()
        {
            audieceWindow.Close();
        }
    }
}
