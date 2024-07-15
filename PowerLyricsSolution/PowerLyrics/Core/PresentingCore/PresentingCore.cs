using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerLyrics.Core.DataHandler;
using PowerLyrics.MVVM.Model;
using PowerLyrics.MVVM.View;
using PowerLyrics.MVVM.ViewModel;

namespace PowerLyrics.Core.PresentingCore;
public class PresentingCore
{
    private MainViewModel _mainViewModel;
    private PresentingViewModel _presentingViewModel;
    private LibraryCore.LibraryCore _libraryCore;

    private PresseningFrom _presentingState;
    
    private readonly DataLoader _songsLoader;
    private readonly DataSaver _songsSaver;
    private readonly TextParser.TextParser _textParser;

    private int _selectedSlide;
    private SongModel? _openedSongModel;

    public PresseningFrom PresentingState
    {
        get => _presentingState;
        set
        {
            _presentingState = value;
            _selectedSlide = -1;
            
            if (value != PresseningFrom.Library)
            {
                //_libraryCore.DeselectSong();
            }
        }
    }
    
    /// <summary>
    /// Opened song
    /// </summary>
    public SongModel? OpenedSongModel
    {
        get => _openedSongModel;
        set
        {
            _openedSongModel = value;
            //If is opened from theese is used text parser to get slides from Library must be used different one
            if (PresentingState is PresseningFrom.None or PresseningFrom.Library or PresseningFrom.File 
                && _openedSongModel is not null)
            {
                _presentingViewModel.DisplayedSlides =
                    _textParser.getSlidesFromOpenSong(_openedSongModel.ContentModels);
            }
        }
    }
    
    public int SelectedSlide
    {
        get => _selectedSlide;
        set => _selectedSlide = value;
    }

    public PresentingCore(MainViewModel pMainViewModel, PresentingViewModel pPresentingViewModel)
    {
        _mainViewModel = pMainViewModel;
        _presentingViewModel = pPresentingViewModel;
        _libraryCore = new(_presentingViewModel);
        
        _songsLoader = new();
        _songsSaver = new();
        _textParser = new();


        InitCore();
    }

    /// <summary>
    /// Init core to the default start up state
    /// </summary>
    private void InitCore()
    {
        PresentingState = PresseningFrom.None;
        _libraryCore.InitLibrary(_songsLoader);
    }

    /// <summary>
    /// Ak true tak sa premieta ak false tak sa nepremieta
    /// </summary>
    /// <param name="pLive"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void GoLive(bool pLive)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Pridá pieseň do playlistu aj z Library ale aj keď je pieseň otvorená zo súboru
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void AddSongToPlayList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Odstráni pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void RemoveSongFromPlayList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Daľšia pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void NextSongInPlaylist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Predcházdajúca pieseň z playlistu
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void PrevSongInPlaylist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Aplykuje edit danej piesne
    /// </summary>
    /// <param name="getEditedSong">Editovaná pieseň</param>
    /// <exception cref="NotImplementedException"></exception>
    public void ApplayEdit(SongModel getEditedSong)
    {
        if (PresentingState is PresseningFrom.None or PresseningFrom.Library or PresseningFrom.File)
        {
            OpenedSongModel = getEditedSong;
        }
        else
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Vráti aktuálne otvorenú pieseň
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public SongModel GetOpenSong()
    {
        return OpenedSongModel != null ? new SongModel(OpenedSongModel) : new SongModel();
    }

    /// <summary>
    /// Otvorý pieseň
    /// </summary>
    /// <param name="o"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OpenSong(object o)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uloží pieseň do súboru
    /// </summary>
    public void SaveSong()
    {
        if (OpenedSongModel != null) _songsSaver.saveSong(OpenedSongModel);
    }

    /// <summary>
    /// Uloží celý playlist
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SavePlaylist()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get song from library and set PreviewControl to blank
    /// </summary>
    /// <param name="index">Index of the song</param>
    public void SelectSongFromLibrary(int index)
    {
        PresentingState = PresseningFrom.Library;
        OpenedSongModel = _libraryCore.GetSong(index);
        SetPriviewControlToBlank();
    }

    /// <summary>
    /// Blank screen for the preview control
    /// </summary>
    public void SetPriviewControlToBlank()
    {
        _presentingViewModel.LyricContent = new LyricViewTemplateText();
    }
}
