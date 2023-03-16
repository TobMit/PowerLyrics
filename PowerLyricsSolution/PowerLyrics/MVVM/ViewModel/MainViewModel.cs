using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PowerLyrics.Core;
using PowerLyrics.Core.DataLoader;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace PowerLyrics.MVVM.ViewModel
{
    public class MainViewModel : ObservableObjects
    {

        private AudiencWindow audieceWindow;

        private DataLoader songsLoader;
        private TextParser textParser;

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
        public RelayCommand test3 { get; set; }
        public RelayCommand SelectSongCommand { get; set; }


        private List<LyricModel> _opendeSong;

        private List<LyricModel> opendeSong
        {
            get
            {
                return _opendeSong;
            }
            set
            {
                _opendeSong = value;
                lyricArray = getSlidesFromOpenSong();
            }
        }

        private ObservableCollection<Slide> _lyricArray;

        public ObservableCollection<Slide> lyricArray
        {
            get
            {
                return _lyricArray;
            }
            set
            {
                _lyricArray = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Song> listOfSongs { get; set; }

        public MainViewModel()
        {
            audieceWindow = new AudiencWindow();
            audieceWindow.Show();

            LyricViewTemplate1 tesLyricViewTemplate1 = new LyricViewTemplate1();
            LyricContent = tesLyricViewTemplate1;
            inicialiseButtons();

            songsLoader = new DataLoader();
            listOfSongs = songsLoader.getSongs();

            textParser = new TextParser();
        }

        private void inicialiseButtons()
        {
            test = new RelayCommand(o =>
            {
                
                Debug.WriteLine("test1");
            });

            test2 = new RelayCommand(o =>
            {
                
                Debug.WriteLine("test2");
            });

            test3 = new RelayCommand(o =>
            {
                Debug.WriteLine(o.ToString());
                LyricContent = new LyricViewTemplate1((LyricViewTemplate1)lyricArray[Int32.Parse(o.ToString())].UserControl);
                
            });
            SelectSongCommand = new RelayCommand(o =>
            {
                Debug.WriteLine(o.ToString());
                opendeSong = textParser.parseLyric(listOfSongs[Int32.Parse(o.ToString())-1]);
            });
        }

        private ObservableCollection<Slide> getSlidesFromOpenSong()
        {
            ObservableCollection<Slide> tmp = new ObservableCollection<Slide>();
            int id = 0;
            LyricType oldType = LyricType.Undefined;
            int oldSerialNumber = 1;
            foreach (var item in opendeSong)
            {
                if (oldType == LyricType.Undefined || oldType != item.LyricType || oldSerialNumber != item.serialNuber)
                {
                    oldType = item.LyricType;
                    oldSerialNumber = item.serialNuber;
                    tmp.Add(new Slide()
                    {
                        SlideType = SlideType.Divider,
                        dividerText =item.serialNuber + ". " + item.LyricType.ToString(),
                    });
                    id++;
                } 
                Slide slide = new Slide();
                slide.UserControl = new LyricViewTemplate1(item);
                slide.id = id;
                slide.SlideType = SlideType.Slide;
                tmp.Add(slide);
                id++;
            }
            return tmp;
        }


        public void closeWindow()
        {
            audieceWindow.Close();
        }
    }
}
