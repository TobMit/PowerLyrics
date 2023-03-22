using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PowerLyrics.Core;
using PowerLyrics.Core.DataLoader;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using WpfScreenHelper;

namespace PowerLyrics.MVVM.ViewModel;

public class PresentingViewModel : ObservableObjects
{
    private AudiencWindow audieceWindow;

    private DataLoader songsLoader;
    private TextParser textParser;
    private bool isLive = false;
    private int selectedSongFromLibrary = -1;
    
    private int _selectedSlide = -1;
    private int selectedSlide
    {
        get
        {
            return _selectedSlide;
        }
        set
        {
            if (value != -1)
            {
                if (_selectedSlide != -1)
                {
                    lyricArray[_selectedSlide].isSelected = false;
                }

                _selectedSlide = value;
                lyricArray[_selectedSlide].isSelected = true;
                lyricArray =
                    new ObservableCollection<Slide>(
                        lyricArray); //! takto to je aby som forsol aktualizaciu obrazovky
            }
            else
            {
                _selectedSlide = value;
            }
        }
            
    }
    private int _selectedSongFromPlaylist = -1;
    private int SelectedSongFromPlaylist
    {
        get
        {
            return _selectedSongFromPlaylist;
        }
        set
        {
                   
            if (_selectedSongFromPlaylist != -1)
            {
                listOfSongsInPlayList[_selectedSongFromPlaylist].isSelected = false;
            }
            _selectedSongFromPlaylist = value;
            listOfSongsInPlayList[_selectedSongFromPlaylist].isSelected = true;
            listOfSongsInPlayList = new ObservableCollection<Song>(listOfSongsInPlayList);
        }
    }
        
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
    public ObservableCollection<Song> listOfSongs { get; set; }

    private ObservableCollection<Song> _listOfSongsInPlayList;
    public ObservableCollection<Song> listOfSongsInPlayList
    {
        get
        {
            return _listOfSongsInPlayList;
        }
        set
        {
            _listOfSongsInPlayList = value;
            OnPropertyChanged();
        }
    }
    public PresentingViewModel()
    {
        audieceWindow = new AudiencWindow();
        audieceWindow.Show();

        LyricViewTemplate1 tesLyricViewTemplate1 = new LyricViewTemplate1("Presun ma na druhe okno");
        LyricContent = tesLyricViewTemplate1;
        inicialiseButtons();

        songsLoader = new DataLoader();
        listOfSongs = songsLoader.getSongs();
        _listOfSongsInPlayList = new ObservableCollection<Song>();


        textParser = new TextParser();
    }
    
    private void inicialiseButtons()
        {
            
            SelectSlideCommand = new RelayCommand(o =>
            {
                selectedSlide = Int32.Parse(o.ToString());
                actualSlidePreviewControl();
            });
            
            SelectLibrarySongCommand = new RelayCommand(o =>
            {
                selectedSongFromLibrary = Int32.Parse(o.ToString());
                openedSong = textParser.parseLyric(listOfSongs[selectedSongFromLibrary]);
                selectedSlide = -1;
                actualSlidePreviewControl();
            });
            
            SelectPlaylistSongCommand = new RelayCommand(o =>
            {
                SelectedSongFromPlaylist = Int32.Parse(o.ToString()); // mam info o id 
                openedSong = textParser.parseLyric(listOfSongsInPlayList[SelectedSongFromPlaylist]);
                selectedSlide = -1;
                actualSlidePreviewControl();
            });
            
        }

    public void PrevSongInPlaylist()
    {
        if (SelectedSongFromPlaylist - 1 >= 0)
        {
            openedSong = textParser.parseLyric(listOfSongsInPlayList[--SelectedSongFromPlaylist]);
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void NextSongInPlaylist()
    {
        if (SelectedSongFromPlaylist + 1 < listOfSongsInPlayList.Count)
        {
            openedSong = textParser.parseLyric(listOfSongsInPlayList[++SelectedSongFromPlaylist]);
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void RemoveSongFromPlayList()
    {
        if (SelectedSongFromPlaylist != -1 && SelectedSongFromPlaylist < listOfSongsInPlayList.Count)
        {
            listOfSongsInPlayList.RemoveAt(SelectedSongFromPlaylist); //todo opravit aby nepadalo
            for (int i = 0; i < listOfSongsInPlayList.Count; i++)
            {
                listOfSongsInPlayList[i].id = i;
            }

            listOfSongsInPlayList = new ObservableCollection<Song>(listOfSongsInPlayList);
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void SetAudienceFullScreanCommand()
    {
        if (Screen.AllScreens.ToArray().Length > 1)
        {
            audieceWindow.setFullScrean();
            actualSlidePreviewControl();
        }
        else
        {
            LyricContent = new LyricViewTemplate1("Pre správne fungovanie potrebujete rozširiť obrazovku.");
        }
    }

    public void GoLive(bool o)
    {
        isLive = o;
        actualSlidePreviewControl();
    }

    public void AddSongToPlayList()
    {
        if (selectedSongFromLibrary != -1)
        {
            listOfSongsInPlayList.Add(listOfSongs[selectedSongFromLibrary]);
            listOfSongsInPlayList[listOfSongsInPlayList.Count - 1].id =
                listOfSongsInPlayList.Count - 1; //aby som vedel spravne mazat z listu
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
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
                slide.isSelected = false;
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