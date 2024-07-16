using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using PowerLyrics.Core;
using PowerLyrics.Core.DataHandler;
using PowerLyrics.Core.PresentingCore;
using PowerLyrics.Core.TextParser;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.Windows;
using WpfScreenHelper;

namespace PowerLyrics.MVVM.ViewModel;

public class PresentingViewModel : ObservableObjects
{
    public PresentingCore PresentingCore;
    private ObservableCollection<SongModel> _listOfLibrarySongs;
    private ObservableCollection<SongModel> _listOfPlaylistSongs;

    private ObservableCollection<Slide> _displayedSlides;

    private LyricViewTemplate _lyricContent;

    private SongModel _openedSongModel;

    private int _selectedSlide = -1;


    private int _selectedSongFromPlaylist = -1;
    private readonly AudiencWindow audieceWindow;
    private bool isLive;
    private int selectedSongFromLibrary = -1;
    private readonly List<SlideSongIndexingModel> SlideSongIndexingModelList;

    //private readonly DataLoader songsLoader;
    private readonly DataSaver songsSaver;
    private readonly TextParser textParser;

    public PresentingViewModel()
    {
        selectedSongFromLibrary = -1;

        audieceWindow = new AudiencWindow();
        audieceWindow.Show();
        
        InitialiseButtons();

        //songsLoader = new DataLoader();
        songsSaver = new DataSaver();
        ListOfLibrarySongs = new();
        _listOfPlaylistSongs = new ObservableCollection<SongModel>();


        textParser = new TextParser();
        DisplayedSlides = new ObservableCollection<Slide>();
        SlideSongIndexingModelList = new List<SlideSongIndexingModel>();
    }

    public PresentingView PresentingView { get; set; }

    /// <summary>
    /// Slide ktorý je aktuálne vybratý
    /// </summary>
    public int SelectedSlide
    {
        get => _selectedSlide;
        set
        {
            if (value != -1)
            {
                if (_selectedSlide != -1 && _selectedSlide <= DisplayedSlides.Count - 1)
                    DisplayedSlides[_selectedSlide].isSelected = false;

                _selectedSlide = value;
                DisplayedSlides[_selectedSlide].isSelected = true;
                setFocus(_selectedSlide);
            }
            else
            {
                _selectedSlide = value;
            }

            PresentingCore.SelectedSlide = value;

            OnPropertyChanged();
        }
    }

    /**
     * Pieseň ktorá je vybratá z playlistu
     */
    private int SelectedSongFromPlaylist
    {
        get => _selectedSongFromPlaylist;
        set
        {
            if (_selectedSongFromPlaylist != -1 && _selectedSongFromPlaylist < ListOfPlaylistSongs.Count)
                ListOfPlaylistSongs[_selectedSongFromPlaylist].isSelected = false;

            _selectedSongFromPlaylist = value;

            // kedze pri vymazavani posuvam ciselnik -1 tak sa to musi osetrit
            if (ListOfPlaylistSongs.Count > 0 && _selectedSongFromPlaylist != -1)
            {
                ListOfPlaylistSongs[_selectedSongFromPlaylist].isSelected = true;
                handleSelectPlaylist();
            }
        }
    }

    /**
     * For audience view and for preview
     */
    public LyricViewTemplate LyricContent
    {
        get => _lyricContent;
        set
        {
            if (value.GetType() == SlideContentType.Video)
            {
                var videoPrew = (LyricViewTemplateVideo) value.Clone();
                var videoAudience = (LyricViewTemplateVideo) value.Clone();
                videoPrew.videoPlayerPlay();
                videoAudience.videoPlayerPlay();
                _lyricContent = videoPrew;
                videoAudience.IsMuted = false;
                audieceWindow.ContentControl.Content = videoAudience;
            }
            else
            {
                _lyricContent = value;
                audieceWindow.ContentControl.Content = value.Clone();
            }
            OnPropertyChanged();
        }
    }

    /**
     * Aktuálne otvorená pieseň
     */
    public SongModel OpenedSongModel
    {
        get => _openedSongModel;
        set
        {
            _openedSongModel = value;
            //DisplayedSlides = textParser.getSlidesFromOpenSong(_openedSongModel.ContentModels);
            //toto je tu kvoli tomu aby sa použil iný parsovač na piesne
            //if (selectedSongFromLibrary > -1)
            //    DisplayedSlides = textParser.getSlidesFromOpenSong(_openedSongModel.ContentModels);

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Dysplayed slides of the song or the playlist
    /// </summary>
    public ObservableCollection<Slide> DisplayedSlides
    {
        get => _displayedSlides;
        set
        {
            _displayedSlides = value;
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
    
    public ObservableCollection<SongModel> ListOfLibrarySongs
    {
        get => _listOfLibrarySongs;
        set
        {
            _listOfLibrarySongs = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Songs in the playlist
    /// </summary>
    public ObservableCollection<SongModel> ListOfPlaylistSongs
    {
        get => _listOfPlaylistSongs;
        set
        {
            _listOfPlaylistSongs = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Button init
    /// </summary>
    private void InitialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o =>
        {
            SelectedSlide = int.Parse(o.ToString());
            if (DisplayedSlides[0].SlideType == SlideType.Divider) handleClickSelectSlidePlaylist();

            actualSlidePreviewControl();
        });

        SelectLibrarySongCommand = new RelayCommand(o =>
        {
            PresentingCore.SelectSongFromLibrary(int.Parse(o.ToString()));
            //selectedSongFromLibrary = int.Parse(o.ToString());
            //OpenedSongModel = new SongModel(ListOfLibrarySongs[selectedSongFromLibrary]);
            //SelectedSlide = -1;
            //SelectedSongFromPlaylist = -1;
            //actualSlidePreviewControl();
        });

        SelectPlaylistSongCommand = new RelayCommand(o =>
        {
            // ak by bol scenár že mám niečo v playliste a potom vyberiem niečo z knižnice a zase kliknem na playlist musim refreshnúť zobrazené slide
            if (DisplayedSlides[0].SlideType != SlideType.Divider)
            {
                SlideSongIndexingModelList.Clear();
                DisplayedSlides = TextParser.getSlidesFromOpenSong(ListOfPlaylistSongs, SlideSongIndexingModelList);
            }

            selectedSongFromLibrary = -1;
            SelectedSongFromPlaylist = int.Parse(o.ToString()); // mam info o id 
            OpenedSongModel = ListOfPlaylistSongs[SelectedSongFromPlaylist];

            actualSlidePreviewControl();
        });
    }

    /**
     * Predchádzajúca pieseň v playliste
     */
    public void PrevSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist - 1 >= 0 && DisplayedSlides[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = ListOfPlaylistSongs[--SelectedSongFromPlaylist];
            actualSlidePreviewControl();
        }
    }

    /**
     * ďaľšia pieseň v playliste
     */
    public void NextSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist + 1 < ListOfPlaylistSongs.Count && DisplayedSlides[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = ListOfPlaylistSongs[++SelectedSongFromPlaylist];
            actualSlidePreviewControl();
        }
    }

    /**
     * Odstráni pieseň z playlistu
     */
    public void RemoveSongFromPlayList()
    {
        if (SelectedSongFromPlaylist != -1 && SelectedSongFromPlaylist < ListOfPlaylistSongs.Count)
        {
            ListOfPlaylistSongs.RemoveAt(SelectedSongFromPlaylist);
            for (var i = 0; i < ListOfPlaylistSongs.Count; i++) ListOfPlaylistSongs[i].id = i;

            SlideSongIndexingModelList.Clear();
            DisplayedSlides = TextParser.getSlidesFromOpenSong(ListOfPlaylistSongs, SlideSongIndexingModelList);

            if (SelectedSongFromPlaylist == ListOfPlaylistSongs.Count)
                SelectedSongFromPlaylist--;
            else
                SelectedSongFromPlaylist = SelectedSongFromPlaylist; // aby som forsol označenie playlistu
            if (ListOfPlaylistSongs.Count <= 0)
            {
                SelectedSlide = -1;
            }
            actualSlidePreviewControl();
        }
    }

    /**
     * Pridá pieseň do playlistu
     */
    public void AddSongToPlayList()
    {
        if (selectedSongFromLibrary != -1)
        {
            ListOfPlaylistSongs.Add(new SongModel(ListOfLibrarySongs[selectedSongFromLibrary]));
            ListOfPlaylistSongs[ListOfPlaylistSongs.Count - 1].id =
                ListOfPlaylistSongs.Count - 1; //aby som vedel spravne mazat z listu

            SlideSongIndexingModelList.Clear();
            DisplayedSlides = TextParser.getSlidesFromOpenSong(ListOfPlaylistSongs, SlideSongIndexingModelList);

            SelectedSongFromPlaylist = ListOfPlaylistSongs.Count - 1;
            actualSlidePreviewControl();
        }
        else if (OpenedSongModel != null)
        {
            ListOfPlaylistSongs.Add(new SongModel(OpenedSongModel));
            ListOfPlaylistSongs[ListOfPlaylistSongs.Count - 1].id =
                ListOfPlaylistSongs.Count - 1; //aby som vedel spravne mazat z listu

            SlideSongIndexingModelList.Clear();
            DisplayedSlides = TextParser.getSlidesFromOpenSong(ListOfPlaylistSongs, SlideSongIndexingModelList);

            SelectedSongFromPlaylist = ListOfPlaylistSongs.Count - 1;
            actualSlidePreviewControl();
        }
    }

    /**
     * Spracovanie kláves
     */
    public void key(KeyEventArgs keyEvent)
    {
        if (keyEvent.Key == Key.Right)
        {
            if (SelectedSlide < DisplayedSlides.Count - 1)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (DisplayedSlides[0].SlideType != SlideType.Divider)
                    SelectedSlide++;
                else
                    handleNextSelectSlidePlaylist();

                actualSlidePreviewControl();
            }
        }
        else if (keyEvent.Key == Key.Left)
        {
            if (SelectedSlide > 0)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (DisplayedSlides[0].SlideType != SlideType.Divider)
                    SelectedSlide--;
                else
                    handlePrevSelectSlidePlaylist();

                actualSlidePreviewControl();
            }
        }
    }

    /**
     * Nastavý audienece okno na druhú obrazovku a do fullscreanu
     */
    public void SetAudienceFullScreanCommand()
    {
        if (Screen.AllScreens.ToArray().Length > 1)
        {
            audieceWindow.setFullScrean();
            actualSlidePreviewControl();
        }
        else
        {
            LyricContent = new LyricViewTemplateText("Pre správne fungovanie potrebujete rozširiť obrazovku.");
        }
    }

    /**
     * Spustí a pozastavý pprezentáciu
     */
    public void GoLive(bool o)
    {
        isLive = o;
        actualSlidePreviewControl();
    }

    /**
     * Riadi čo je zobrazené v preview a u divákov
     */
    private void actualSlidePreviewControl()
    {
        if (isLive)
        {
            // pre istotu ak by sa sem dostal nejakou náhodov divider ktorý sa nedá zobraziť
            if (SelectedSlide != -1 && DisplayedSlides[SelectedSlide].SlideType != SlideType.Divider)
                LyricContent = (LyricViewTemplate)DisplayedSlides[SelectedSlide].UserControl.Clone();
            else
                LyricContent = new LyricViewTemplateText();
        }
        else
        {
            LyricContent = new LyricViewTemplateText(constants.DEFAULT_TEXT);
        }
    }

    /**
     * Načíta pieseň, playlist zo súboru
     */
    public void OpenSong(string? path)
    {
        // if (path != null)
        //     songsLoader.loadFileStartUp(path);
        // else
        //     songsLoader.loadFile();
        // switch (songsLoader.openedFileType)
        // {
        //     case FileType.Song:
        //         OpenedSongModel = songsLoader.getSongModel();
        //         DisplayedSlides = textParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
        //         break;
        //     case FileType.PlayList:
        //         listOfSongsInPlayList = songsLoader.getPlaylist();
        //         SlideSongIndexingModelList.Clear();
        //         DisplayedSlides = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
        //         SelectedSongFromPlaylist = 0;
        //         handleSelectPlaylist();
        //         break;
        //     default:
        //         break;
        // }
    }

    /**
     * Uloží pieseň do súboru
     */
    public void SaveSong()
    {
        if (OpenedSongModel != null) songsSaver.saveSong(OpenedSongModel);
    }

    /**
     * Uloží playlist do súboru
     */
    public void SavePlaylist()
    {
        if (ListOfPlaylistSongs.Count > 0) songsSaver.savePlaylist(ListOfPlaylistSongs);
    }

    /**
     * Zatvorí okno pre divákov
     */
    public void closeWindow()
    {
        audieceWindow.Close();
    }

    /**
     * Vráti otvorenú pieseň, ak nie je otvorená pieseň tak vráti prázdnu pieseň
     */
    public SongModel getOpenSong()
    {
        return OpenedSongModel != null ? new SongModel(OpenedSongModel) : new SongModel();
    }

    /**
     * aplykuje edit z edit page
     */
    public void applayEdit(SongModel songModel)
    {
        if (DisplayedSlides.Count != 0)
        {
            if (DisplayedSlides[0].SlideType == SlideType.Divider)
            {
                OpenedSongModel = songModel;
                ListOfPlaylistSongs[SelectedSongFromPlaylist] = songModel;
                SlideSongIndexingModelList.Clear();
                DisplayedSlides = TextParser.getSlidesFromOpenSong(ListOfPlaylistSongs, SlideSongIndexingModelList);
                SelectedSongFromPlaylist = SelectedSongFromPlaylist;
            }
            else
            {
                OpenedSongModel = songModel;
                DisplayedSlides = TextParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
            }
        }
        else
        {
            OpenedSongModel = songModel;
            DisplayedSlides = TextParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
        }

        selectedSongFromLibrary = -1;
    }

    /**
     * Nastaví focus pre správne používanie klávesnice
     */
    private void setFocus(int index)
    {
        var item =
            PresentingView.ListViewSlides.ItemContainerGenerator.ContainerFromIndex(_selectedSlide) as ListViewItem;
        item.Focus();
    }

    /**
     * Riadene select v playliste
     */
    private void handleSelectPlaylist()
    {
        SelectedSlide = SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide;
    }

    /**
     * ďaľší slide v playliste
     */
    private void handleNextSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (SelectedSlide < SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfLastSlide)
            SelectedSlide++;
        else
            NextSongInPlaylist();
    }

    /**
     * Predchádzajúci slide v playliste
     */
    private void handlePrevSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (SelectedSlide > SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide)
            SelectedSlide--;
        else
            PrevSongInPlaylist();
    }

    /**
     * Vyber slide pomocou myšli v playliste
     */
    private void handleClickSelectSlidePlaylist()
    {
        for (var i = 0; i < SlideSongIndexingModelList.Count; i++)
            if (SelectedSlide >= SlideSongIndexingModelList[i].indexOfFirstSlide &&
                SelectedSlide <= SlideSongIndexingModelList[i].indexOfLastSlide)
            {
                var tmpSelectedSlide = SelectedSlide;
                SelectedSongFromPlaylist = i;
                SelectedSlide = tmpSelectedSlide;
                selectedSongFromLibrary = -1;
                OpenedSongModel = new SongModel(ListOfPlaylistSongs[SelectedSongFromPlaylist]);
                break;
            }
    }
}