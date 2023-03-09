using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerLyrics.Core;
using PowerLyrics.MVVM.Model;
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

        //toto je tu kvoli tomu aby som jednoduhsie nastavoval text v labeli
        private string actualLabelText
        {
            set
            {
                LyricViewTemplate1 tmp = new LyricViewTemplate1();
                tmp.Label.Content = value;
                LyricContent = tmp;
            }
        }

        public RelayCommand test { get; set; }
        public RelayCommand test2 { get; set; }

        public ObservableCollection<LyricModel> lyricArray { get; set; }

        public MainViewModel()
        {
            audieceWindow = new AudiencWindow();
            audieceWindow.Show();
            LyricViewTemplate1 tesLyricViewTemplate1 = new LyricViewTemplate1();
            LyricContent = tesLyricViewTemplate1;
            inicialiseButtons();

            lyricArray = new ObservableCollection<LyricModel>();
            
        }

        private void inicialiseButtons()
        {
            test = new RelayCommand(o =>
            {
                actualLabelText = "test";
                lyricArray.Clear();
                for (int i = 0; i < 70; i++)
                {
                    lyricArray.Add(new LyricModel() { text = "test" + i });
                }
            });

            test2 = new RelayCommand(o =>
            {
                actualLabelText = "Toto je test";
                lyricArray.Clear();
                for (int i = 0; i < 70; i++)
                {
                    lyricArray.Add(new LyricModel() { text = "Toto je test" + i });
                }
            });
        }


        public void closeWindow()
        {
            audieceWindow.Close();
        }
    }
}
