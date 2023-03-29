using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
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
    public PresentingView PresentingView { get; set; }

    private int _selectedSlide = -1;

    public int selectedSlide
    {
        get { return _selectedSlide; }
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
                ListViewItem item = PresentingView.ListViewSlides.ItemContainerGenerator.ContainerFromIndex(_selectedSlide) as ListViewItem;
                item.Focus();

            }
            else
            {
                _selectedSlide = value;
            }
            OnPropertyChanged();
        }
    }

    private int _selectedSongFromPlaylist = -1;

    private int SelectedSongFromPlaylist
    {
        get { return _selectedSongFromPlaylist; }
        set
        {
            if (_selectedSongFromPlaylist != -1 && _selectedSongFromPlaylist < listOfSongsInPlayList.Count)
            {
                listOfSongsInPlayList[_selectedSongFromPlaylist].isSelected = false;
            }

            _selectedSongFromPlaylist = value;

            // kedze pri vymazavani posuvam ciselnik -1 tak sa to musi osetrit
            if (listOfSongsInPlayList.Count > 0 && _selectedSongFromPlaylist != -1)
            {
                listOfSongsInPlayList[_selectedSongFromPlaylist].isSelected = true;
            }
            
        }
    }

    /**
     * For audience view and for preview
     */
    private object _lyricContent;

    public object LyricContent
    {
        get { return _lyricContent; }
        set
        {
            _lyricContent = value;
            audieceWindow.ContentControl.Content = new LyricViewTemplate1((LyricViewTemplate1)LyricContent);
            OnPropertyChanged();
        }
    }

    private SongModel _openedSongModel;

    public SongModel OpenedSongModel
    {
        get { return _openedSongModel; }
        set
        {
            _openedSongModel = value;
            lyricArray = textParser.getSlidesFromOpenSong(_openedSongModel.LyricModels);
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Slide> _lyricArray;

    public ObservableCollection<Slide> lyricArray
    {
        get { return _lyricArray; }
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

    public ObservableCollection<SongModel> listOfSongs { get; set; }

    private ObservableCollection<SongModel> _listOfSongsInPlayList;

    public ObservableCollection<SongModel> listOfSongsInPlayList
    {
        get { return _listOfSongsInPlayList; }
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

        LyricViewTemplate1 tesLyricViewTemplate1 =
            new LyricViewTemplate1("Pre začatie prezentovania stlačte Fullsc tlačídko!");
        LyricContent = tesLyricViewTemplate1;
        inicialiseButtons();

        songsLoader = new DataLoader();
        listOfSongs = songsLoader.getSongs();
        _listOfSongsInPlayList = new ObservableCollection<SongModel>();


        textParser = new TextParser();
        lyricArray = new ObservableCollection<Slide>();
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
            OpenedSongModel = new SongModel(listOfSongs[selectedSongFromLibrary]);
            selectedSlide = -1;
            SelectedSongFromPlaylist = -1;
            actualSlidePreviewControl();
        });

        SelectPlaylistSongCommand = new RelayCommand(o =>
        {
            SelectedSongFromPlaylist = Int32.Parse(o.ToString()); // mam info o id 
            OpenedSongModel = listOfSongsInPlayList[SelectedSongFromPlaylist];
            selectedSlide = -1;
            actualSlidePreviewControl();
        });
    }

    public void PrevSongInPlaylist()
    {
        if (SelectedSongFromPlaylist - 1 >= 0)
        {
            OpenedSongModel = listOfSongsInPlayList[--SelectedSongFromPlaylist];
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void NextSongInPlaylist()
    {
        if (SelectedSongFromPlaylist + 1 < listOfSongsInPlayList.Count)
        {
            OpenedSongModel = listOfSongsInPlayList[++SelectedSongFromPlaylist];
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void RemoveSongFromPlayList()
    {
        if (SelectedSongFromPlaylist != -1 && SelectedSongFromPlaylist < listOfSongsInPlayList.Count)
        {
            listOfSongsInPlayList.RemoveAt(SelectedSongFromPlaylist);
            for (int i = 0; i < listOfSongsInPlayList.Count; i++)
            {
                listOfSongsInPlayList[i].id = i;
            }

            listOfSongsInPlayList = new ObservableCollection<SongModel>(listOfSongsInPlayList);
            selectedSlide = -1;
            actualSlidePreviewControl();
        }
    }

    public void AddSongToPlayList()
    {
        if (selectedSongFromLibrary != -1)
        {
            listOfSongsInPlayList.Add(new SongModel(listOfSongs[selectedSongFromLibrary]));
            listOfSongsInPlayList[listOfSongsInPlayList.Count - 1].id =
                listOfSongsInPlayList.Count - 1; //aby som vedel spravne mazat z listu
            selectedSlide = -1;
            SelectedSongFromPlaylist = listOfSongsInPlayList.Count - 1;
            actualSlidePreviewControl();
        } else if (OpenedSongModel != null)
        {
            listOfSongsInPlayList.Add(new SongModel(OpenedSongModel));
            listOfSongsInPlayList[listOfSongsInPlayList.Count - 1].id =
                listOfSongsInPlayList.Count - 1; //aby som vedel spravne mazat z listu
            selectedSlide = -1;
            SelectedSongFromPlaylist = listOfSongsInPlayList.Count - 1;
            actualSlidePreviewControl();
        }
    }

    public void key(KeyEventArgs keyEvent)
    {
        if (keyEvent.Key == Key.Right)
        {
            if (selectedSlide < lyricArray.Count -1)
            {
                selectedSlide++;
                actualSlidePreviewControl();
            } 
        } else if (keyEvent.Key == Key.Left)
        {
            if (selectedSlide > 0)
            {
                selectedSlide--;
                actualSlidePreviewControl();
            }
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

    public SongModel getOpenSong()
    {
        return OpenedSongModel != null ? new SongModel(OpenedSongModel) : new SongModel();
    }

    public void applayEdit(SongModel songModel)
    {
        OpenedSongModel = songModel;
        if (SelectedSongFromPlaylist != -1 && listOfSongsInPlayList.Count > 0)
        {
            listOfSongsInPlayList[SelectedSongFromPlaylist] = songModel;
        }
    }

}