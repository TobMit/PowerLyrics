﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    private ObservableCollection<SongModel> _listOfSongsInPlayList;

    private ObservableCollection<Slide> _lyricArray;

    private LyricViewTemplate _lyricContent;

    private SongModel _openedSongModel;

    private int _selectedSlide = -1;


    private int _selectedSongFromPlaylist = -1;
    private readonly AudiencWindow audieceWindow;
    private bool isLive;
    private int selectedSongFromLibrary = -1;
    private readonly List<SlideSongIndexingModel> SlideSongIndexingModelList;

    private readonly DataLoader songsLoader;
    private readonly DataSaver songsSaver;
    private readonly TextParser textParser;

    public PresentingViewModel()
    {
        selectedSongFromLibrary = -1;

        audieceWindow = new AudiencWindow();
        audieceWindow.Show();

        var tesLyricViewTemplate1 =
            new LyricViewTemplateText("Pre začatie prezentovania stlačte Fullsc tlačídko!");
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

    public PresentingView PresentingView { get; set; }

    /**
     * Slide ktorý je aktuálne vybratý
     */
    public int selectedSlide
    {
        get => _selectedSlide;
        set
        {
            if (value != -1)
            {
                if (_selectedSlide != -1 && _selectedSlide <= lyricArray.Count - 1)
                    lyricArray[_selectedSlide].isSelected = false;

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

    /**
     * Pieseň ktorá je vybratá z playlistu
     */
    private int SelectedSongFromPlaylist
    {
        get => _selectedSongFromPlaylist;
        set
        {
            if (_selectedSongFromPlaylist != -1 && _selectedSongFromPlaylist < listOfSongsInPlayList.Count)
                listOfSongsInPlayList[_selectedSongFromPlaylist].isSelected = false;

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
            //toto je tu kvoli tomu aby sa použil iný parsovač na piesne
            if (selectedSongFromLibrary > -1)
                lyricArray = textParser.getSlidesFromOpenSong(_openedSongModel.ContentModels);

            OnPropertyChanged();
        }
    }

    /**
     * Slides ktoré sú zobrazované a z ktorých sa vyberá
     */
    public ObservableCollection<Slide> lyricArray
    {
        get => _lyricArray;
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

    /**
     * Piesne ktoré sa nachadzajú v playliste
     */
    public ObservableCollection<SongModel> listOfSongsInPlayList
    {
        get => _listOfSongsInPlayList;
        set
        {
            _listOfSongsInPlayList = value;
            OnPropertyChanged();
        }
    }

    /**
     * Inicializácie funkcionality tlačidiel
     */
    private void inicialiseButtons()
    {
        SelectSlideCommand = new RelayCommand(o =>
        {
            selectedSlide = int.Parse(o.ToString());
            if (lyricArray[0].SlideType == SlideType.Divider) handleClickSelectSlidePlaylist();

            actualSlidePreviewControl();
        });

        SelectLibrarySongCommand = new RelayCommand(o =>
        {
            selectedSongFromLibrary = int.Parse(o.ToString());
            OpenedSongModel = new SongModel(listOfSongs[selectedSongFromLibrary]);
            selectedSlide = -1;
            SelectedSongFromPlaylist = -1;
            actualSlidePreviewControl();
        });

        SelectPlaylistSongCommand = new RelayCommand(o =>
        {
            // ak by bol scenár že mám niečo v playliste a potom vyberiem niečo z knižnice a zase kliknem na playlist musim refreshnúť zobrazené slide
            if (lyricArray[0].SlideType != SlideType.Divider)
            {
                SlideSongIndexingModelList.Clear();
                lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
            }

            selectedSongFromLibrary = -1;
            SelectedSongFromPlaylist = int.Parse(o.ToString()); // mam info o id 
            OpenedSongModel = listOfSongsInPlayList[SelectedSongFromPlaylist];

            actualSlidePreviewControl();
        });
    }

    /**
     * Predchádzajúca pieseň v playliste
     */
    public void PrevSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist - 1 >= 0 && lyricArray[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = listOfSongsInPlayList[--SelectedSongFromPlaylist];
            actualSlidePreviewControl();
        }
    }

    /**
     * ďaľšia pieseň v playliste
     */
    public void NextSongInPlaylist()
    {
        // ak je prvý slide divider tak sa zobrazuje playlist
        if (SelectedSongFromPlaylist + 1 < listOfSongsInPlayList.Count && lyricArray[0].SlideType == SlideType.Divider)
        {
            OpenedSongModel = listOfSongsInPlayList[++SelectedSongFromPlaylist];
            actualSlidePreviewControl();
        }
    }

    /**
     * Odstráni pieseň z playlistu
     */
    public void RemoveSongFromPlayList()
    {
        if (SelectedSongFromPlaylist != -1 && SelectedSongFromPlaylist < listOfSongsInPlayList.Count)
        {
            listOfSongsInPlayList.RemoveAt(SelectedSongFromPlaylist);
            for (var i = 0; i < listOfSongsInPlayList.Count; i++) listOfSongsInPlayList[i].id = i;

            SlideSongIndexingModelList.Clear();
            lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);

            if (SelectedSongFromPlaylist == listOfSongsInPlayList.Count)
                SelectedSongFromPlaylist--;
            else
                SelectedSongFromPlaylist = SelectedSongFromPlaylist; // aby som forsol označenie playlistu
            if (listOfSongsInPlayList.Count <= 0)
            {
                selectedSlide = -1;
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

    /**
     * Spracovanie kláves
     */
    public void key(KeyEventArgs keyEvent)
    {
        if (keyEvent.Key == Key.Right)
        {
            if (selectedSlide < lyricArray.Count - 1)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (lyricArray[0].SlideType != SlideType.Divider)
                    selectedSlide++;
                else
                    handleNextSelectSlidePlaylist();

                actualSlidePreviewControl();
            }
        }
        else if (keyEvent.Key == Key.Left)
        {
            if (selectedSlide > 0)
            {
                // ak je prvy divider zobrazuje sa playlist
                if (lyricArray[0].SlideType != SlideType.Divider)
                    selectedSlide--;
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
            if (selectedSlide != -1 && lyricArray[selectedSlide].SlideType != SlideType.Divider)
                LyricContent = (LyricViewTemplate)lyricArray[selectedSlide].UserControl.Clone();
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
        if (path != null)
            songsLoader.loadFileStartUp(path);
        else
            songsLoader.loadFile();
        switch (songsLoader.openedFileType)
        {
            case FileType.Song:
                OpenedSongModel = songsLoader.getSongModel();
                lyricArray = textParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
                break;
            case FileType.PlayList:
                listOfSongsInPlayList = songsLoader.getPlaylist();
                SlideSongIndexingModelList.Clear();
                lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
                SelectedSongFromPlaylist = 0;
                handleSelectPlaylist();
                break;
            default:
                break;
        }
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
        if (listOfSongsInPlayList.Count > 0) songsSaver.savePlaylist(listOfSongsInPlayList);
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
        if (lyricArray.Count != 0)
        {
            if (lyricArray[0].SlideType == SlideType.Divider)
            {
                OpenedSongModel = songModel;
                listOfSongsInPlayList[SelectedSongFromPlaylist] = songModel;
                SlideSongIndexingModelList.Clear();
                lyricArray = textParser.getSlidesFromOpenSong(listOfSongsInPlayList, SlideSongIndexingModelList);
                SelectedSongFromPlaylist = SelectedSongFromPlaylist;
            }
            else
            {
                OpenedSongModel = songModel;
                lyricArray = textParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
            }
        }
        else
        {
            OpenedSongModel = songModel;
            lyricArray = textParser.getSlidesFromOpenSong(OpenedSongModel.ContentModels);
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
        selectedSlide = SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide;
    }

    /**
     * ďaľší slide v playliste
     */
    private void handleNextSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (selectedSlide < SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfLastSlide)
            selectedSlide++;
        else
            NextSongInPlaylist();
    }

    /**
     * Predchádzajúci slide v playliste
     */
    private void handlePrevSelectSlidePlaylist()
    {
        selectedSongFromLibrary = -1;
        if (selectedSlide > SlideSongIndexingModelList[SelectedSongFromPlaylist].indexOfFirstSlide)
            selectedSlide--;
        else
            PrevSongInPlaylist();
    }

    /**
     * Vyber slide pomocou myšli v playliste
     */
    private void handleClickSelectSlidePlaylist()
    {
        for (var i = 0; i < SlideSongIndexingModelList.Count; i++)
            if (selectedSlide >= SlideSongIndexingModelList[i].indexOfFirstSlide &&
                selectedSlide <= SlideSongIndexingModelList[i].indexOfLastSlide)
            {
                var tmpSelectedSlide = selectedSlide;
                SelectedSongFromPlaylist = i;
                selectedSlide = tmpSelectedSlide;
                selectedSongFromLibrary = -1;
                OpenedSongModel = new SongModel(listOfSongsInPlayList[SelectedSongFromPlaylist]);
                break;
            }
    }
}