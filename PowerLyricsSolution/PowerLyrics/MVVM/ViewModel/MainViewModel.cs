using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.VisualBasic;
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
        private bool isLive = false;
        private int selectedSlide = -1;
        private int selectedSongFromLibrary = -1;
        private int selectedSongFromPlaylist = -1;
        
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
        /**
         * Set audience window full screan
         */
        public RelayCommand SetAudenceFullScreanCommand { get; set; }
        /**
         * Set status to live
         */
        public RelayCommand GoLiveCommand { get; set; }
        /**
         * select slide form slide prewiew
         */
        public RelayCommand SelectSlideCommand { get; set; }
        /**
         * Select song from library
         */
        public RelayCommand SelectLibrarySongCommand { get; set; }
        /**
         * Select song from playlist
         */
        public RelayCommand SelectPlaylistSongCommand { get; set; }
        /**
         * Add song from library to playlist
         */
        public RelayCommand AddSongToPlayListCommand { get; set; }
        /**
         * Remove song from playlist
         */
        public RelayCommand RemoveSongFromPlayListCommand { get; set; }
        /**
         * Fast forward selected slide to first slide on next song
         */
        public RelayCommand NextSongInPlaylistCommand { get; set; }
        /**
         * Fast rewind selected slide to first slide previous song
         */
        public RelayCommand PrewSongInPlaylistCommand { get; set; }


        private List<LyricModel> _openedSong;

        private List<LyricModel> openedSong
        {
            get
            {
                return _openedSong;
            }
            set
            {
                _openedSong = value;
                lyricArray = getSlidesFromOpenSong();
                OnPropertyChanged();
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
        public ObservableCollection<Song> listOfSongsInPlayList { get; set; }

        public MainViewModel()
        {
            audieceWindow = new AudiencWindow();
            audieceWindow.Show();

            LyricViewTemplate1 tesLyricViewTemplate1 = new LyricViewTemplate1("Presun ma na druhe okno");
            LyricContent = tesLyricViewTemplate1;
            inicialiseButtons();

            songsLoader = new DataLoader();
            listOfSongs = songsLoader.getSongs();
            listOfSongsInPlayList = new ObservableCollection<Song>();

            textParser = new TextParser();
        }

        private void inicialiseButtons()
        {
            
            SetAudenceFullScreanCommand = new RelayCommand(o =>
            {
                audieceWindow.setFullScrean();
                actualSlidePreviewControl();
            });

            GoLiveCommand = new RelayCommand(o =>
            {
                isLive = (bool)o;
                actualSlidePreviewControl();
            });
            
            SelectSlideCommand = new RelayCommand(o =>
            {
                selectedSlide = Int32.Parse(o.ToString());
                actualSlidePreviewControl();
            });
            
            SelectLibrarySongCommand = new RelayCommand(o =>
            {
                selectedSongFromLibrary = Int32.Parse(o.ToString()) - 1;
                openedSong = textParser.parseLyric(listOfSongs[selectedSongFromLibrary]);
                selectedSlide = -1;
                actualSlidePreviewControl();
            });
            
            SelectPlaylistSongCommand = new RelayCommand(o =>
            {
                selectedSongFromPlaylist = Int32.Parse(o.ToString()) - 1;
                openedSong = textParser.parseLyric(listOfSongs[selectedSongFromPlaylist]);
                selectedSlide = -1;
                actualSlidePreviewControl();
            });
            
            AddSongToPlayListCommand = new RelayCommand(o =>
            {
                if (selectedSongFromLibrary != -1)
                {
                    listOfSongsInPlayList.Add(listOfSongs[selectedSongFromLibrary]);
                    selectedSlide = -1;
                    actualSlidePreviewControl();
                }
            });
            RemoveSongFromPlayListCommand = new RelayCommand(o =>
            {
                if (selectedSongFromPlaylist != -1 && selectedSongFromPlaylist < listOfSongsInPlayList.Count)
                {
                    listOfSongsInPlayList.RemoveAt(selectedSongFromPlaylist); //todo opravit aby nepadalo
                    selectedSlide = -1;
                    actualSlidePreviewControl();
                }
            });
            //todo pridat este forward a prev + atribút na označenie ktorý slide je selected v triede Song a slide aby sa spravil design
            
            
        }

        private ObservableCollection<Slide> getSlidesFromOpenSong()
        {
            ObservableCollection<Slide> tmp = new ObservableCollection<Slide>();
            int id = 0;
            LyricType oldType = LyricType.Undefined;
            int oldSerialNumber = 1;
            foreach (var item in openedSong)
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

        private void actualSlidePreviewControl()
        {
            if (isLive)                                                                                                            
            {                                                                                                                      
                if (selectedSlide != -1)                                                                                          
                {                                                                                                                  
                    LyricContent = new LyricViewTemplate1((LyricViewTemplate1)lyricArray[selectedSlide].UserControl);  
                }                                                                                                                  
                else                                                                                                               
                {                                                                                                                  
                    LyricContent = new LyricViewTemplate1();                                                                       
                }                                                                                                                  
            }                                                                                                                      
            else                                                                                                                   
            {                                                                                                                      
                LyricContent = new LyricViewTemplate1(constants.DEFAULT_TEXT);
            }                                                                                                                      
        }


        public void closeWindow()
        {
            audieceWindow.Close();
        }
    }
}
