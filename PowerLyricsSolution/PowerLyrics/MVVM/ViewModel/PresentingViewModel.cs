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
using PowerLyrics.Core.DataHandler;
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
    private DataSaver songsSaver;
    private TextParser textParser;
    private bool isLive = false;
    private int selectedSongFromLibrary = -1;
    public PresentingView PresentingView { get; set; }
    private List<SlideSongIndexingModel> SlideSongIndexingModelList;

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
                setFocus(_selectedSlide);
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
                handleSelectPlaylist();
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
            //toto je tu kvoli tomu aby sa použil iný parsovač na piesne
            if (selectedSongFromLibrary > -1)
            {
                lyricArray = textParser.getSlidesFromOpenSong(_openedSongModel.LyricModels);
            }

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
        selectedSongFromLibrary = -1;

        audieceWindow = new AudiencWindow();
        audieceWindow.Show();

        LyricViewTemplate1 tesLyricViewTemplate1 =
            new LyricViewTemplate1("Pre začatie prezentovania stlačte Fullsc tlačídko!");
        LyricContent = tesLyricViewTemplate1;
        inicialiseButtons();

        songsLoader = new DataLoader();
        songsSaver = new DataSaver();
        listOfSongs = songsLoader.getSongs();
        _listOfSongsInPlayList = new ObservableCollection<SongModel>();


        textParser = new TextParser();
        lyricArray = new ObservableCollection<Slide>();
        SlideSongIndexingModelList = new List<SlideSongIndexingModel>();
    }

    private void inicialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o =>
        {
            selectedSlide = Int32.Parse(o.ToString());
            if (lyricArray[0].SlideType == SlideType.Divider)
            {
                handleClickSelectSlidePlaylist();
            }

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
            selectedSongFromLibrary = -1;
            // ak by bol scenár že mám niečo v playliste a potom vyberiem niečo z knižnice a zase kliknem na playlist musim refreshnúť zobrazené slide
            if (lyricArray[0].SlideType != SlideType.Divider)
            {
                SlideSongIndexingModelList.Clear();
                lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
            }

            actualSlidePreviewControl();
        });
    }

    public void PrevSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist - 1 >= 0 && lyricArray[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = listOfSongsInPlayList[--SelectedSongFromPlaylist];
            actualSlidePreviewControl();
        }
    }

    public void NextSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist + 1 < listOfSongsInPlayList.Count && lyricArray[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = listOfSongsInPlayList[++SelectedSongFromPlaylist];
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

            //listOfSongsInPlayList = new ObservableCollection<SongModel>(listOfSongsInPlayList);
            selectedSlide = -1;
            actualSlidePreviewControl();

            SlideSongIndexingModelList.Clear();
            lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
        }
    }

    public void AddSongToPlayList()
    {
        if (selectedSongFromLibrary != -1)
        {
            listOfSongsInPlayList.Add(new SongModel(listOfSongs[selectedSongFromLibrary]));
            listOfSongsInPlayList[listOfSongsInPlayList.Count - 1].id =
                listOfSongsInPlayList.Count - 1; //aby som vedel spravne mazat z listu

            SlideSongIndexingModelList.Clear();
            lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);

            SelectedSongFromPlaylist = listOfSongsInPlayList.Count - 1;
            actualSlidePreviewControl();
        }
        else if (OpenedSongModel != null)
        {
            listOfSongsInPlayList.Add(new SongModel(OpenedSongModel));
            listOfSongsInPlayList[listOfSongsInPlayList.Count - 1].id =
                listOfSongsInPlayList.Count - 1; //aby som vedel spravne mazat z listu

            SlideSongIndexingModelList.Clear();
            lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);

            SelectedSongFromPlaylist = listOfSongsInPlayList.Count - 1;
            actualSlidePreviewControl();
        }
    }

    public void key(KeyEventArgs keyEvent)
    {
        if (keyEvent.Key == Key.Right)
        {
            if (selectedSlide < lyricArray.Count - 1)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (lyricArray[0].SlideType != SlideType.Divider)
                {
                    selectedSlide++;
                }
                else
                {
                    handleNextSelectSlidePlaylist();
                }

                actualSlidePreviewControl();
            }
        }
        else if (keyEvent.Key == Key.Left)
        {
            if (selectedSlide > 0)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (lyricArray[0].SlideType != SlideType.Divider)
                {
                    selectedSlide--;
                }
                else
                {
                    handlePrevSelectSlidePlaylist();
                }

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
            // pre istotu ak by sa sem dostal nejakou náhodov divider ktorý sa nedá zobraziť
            if (selectedSlide != -1 && lyricArray[selectedSlide].SlideType != SlideType.Divider)
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

    public void OpenSong()
    {
        songsLoader.loadFile();
        switch (songsLoader.openedFileType)
        {
            case FileType.Song:
                OpenedSongModel = songsLoader.getSongModel();
                lyricArray = textParser.getSlidesFromOpenSong(OpenedSongModel.LyricModels);
                break;
        }
    }

    public void SaveSong()
    {
        if (OpenedSongModel != null)
        {
            this.songsSaver.saveSong(OpenedSongModel);
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
        if (lyricArray.Count != 0)
        {
            if (lyricArray[0].SlideType == SlideType.Divider)
            {
                listOfSongsInPlayList[SelectedSongFromPlaylist] = songModel;
                SlideSongIndexingModelList.Clear();
                lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
            }
        }

        selectedSongFromLibrary = -1;
    }

    private void setFocus(int index)
    {
        ListViewItem item =
            PresentingView.ListViewSlides.ItemContainerGenerator.ContainerFromIndex(_selectedSlide) as ListViewItem;
        item.Focus();
    }

    private void handleSelectPlaylist()
    {
        selectedSlide = SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide;
    }

    private void handleNextSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (selectedSlide < SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfLastSlide)
        {
            selectedSlide++;
        }
        else
        {
            NextSongInPlaylist();
        }
    }

    private void handlePrevSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (selectedSlide > SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide)
        {
            selectedSlide--;
        }
        else
        {
            PrevSongInPlaylist();
        }
    }

    private void handleClickSelectSlidePlaylist()
    {
        for (int i = 0; i < SlideSongIndexingModelList.Count; i++)
        {
            if (selectedSlide >= SlideSongIndexingModelList[i].indexOfFirstSlide &&
                selectedSlide <= SlideSongIndexingModelList[i].indexOfLastSlide)
            {
                int tmpSelectedSlide = selectedSlide;
                SelectedSongFromPlaylist = i;
                selectedSlide = tmpSelectedSlide;
                break;
            }
        }
    }
}